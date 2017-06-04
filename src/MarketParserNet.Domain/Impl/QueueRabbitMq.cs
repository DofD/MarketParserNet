using System;
using System.Collections.Concurrent;
using System.Threading;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Адаптер очереди RabbitMQ
    /// </summary>
    public class QueueRabbitMq : IQueue, IDisposable
    {
        /// <summary>
        ///     Модель AMQP
        /// </summary>
        private readonly IModel _channel;

        /// <summary>
        ///     Конфигурация очереди
        /// </summary>
        private readonly IConfigurationRabbitMq _config;

        /// <summary>
        ///     Соединение
        /// </summary>
        private readonly IConnection _connection;

        /// <summary>
        ///     Фабрика соединений
        /// </summary>
        private readonly ConnectionFactory _factory;

        /// <summary>
        ///     Логировщик
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     Объект блокировки модели
        /// </summary>
        private readonly object _modelLock = new object();

        /// <summary>
        ///     Подписчики
        /// </summary>
        private readonly ConcurrentDictionary<string, IConnection> _observers =
            new ConcurrentDictionary<string, IConnection>();

        /// <summary>
        ///     Блокировка подписчиков
        /// </summary>
        private readonly ReaderWriterLockSlim _observersLock = new ReaderWriterLockSlim();

        /// <summary>
        ///     Сериализатор
        /// </summary>
        private readonly ISerializer _serializer;

        /// <summary>
        ///     Флаг очистки ресурсов
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="QueueRabbitMq" />
        /// </summary>
        /// <param name="factory">Фабрика подключения</param>
        /// <param name="configManager">Менеджер конфигурации</param>
        /// <param name="serializer">Сериализатор</param>
        /// <param name="logger">Логировщик</param>
        public QueueRabbitMq(
            ConnectionFactory factory,
            IConfigManager<IConfigurationRabbitMq> configManager,
            ISerializer serializer,
            ILogger logger)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this._factory = factory;
            this._logger = logger;
            this._serializer = serializer;

            this._config = configManager.GetConfiguration();

            this._connection = this.Connect();

            this._channel = this.GetСhannel(this._connection);
        }

        /// <summary>
        ///     Выполнить определяемые приложением задачи, связанные с высвобождением или сбросом неуправляемых ресурсов
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Вернуть актуальный элемент очереди
        /// </summary>
        /// <param name="routingKey">Маска</param>
        /// <returns>Элемент очереди</returns>
        public QueueMessage Dequeue(string routingKey)
        {
            var queueFullName = GetQueueFullName(routingKey, this._config.ExchangeName);

            try
            {
                var receive = this._channel.BasicGet(queueFullName, false);
                if (receive == null)
                {
                    return null;
                }

                var message = this._serializer.Deserialize<QueueMessage>(receive.Body);

                this._channel.BasicAck(receive.DeliveryTag, false);

                return message;
            }
            catch (Exception e)
            {
                this._logger.Error("Ошибка получения элемента из очереди", e);
                throw;
            }
        }

        /// <summary>
        ///     Добавить новый элемент в очередь
        /// </summary>
        /// <param name="item">Добавляемый элемент</param>
        /// <param name="routingKey">Маска</param>
        public void Enqueue(QueueMessage item, string routingKey)
        {
            // Создадим базовые параметры
            var basicProperties = this._channel.CreateBasicProperties();

            // Говорим что сообщение постоянное
            basicProperties.Persistent = true;

            // блокируем модель иначе может вызваться исключение при много поточности
            lock (this._modelLock)
            {
                try
                {
                    // Инициализируем объекты RabbitMq
                    this.InitializeRabbitMq(routingKey, this._channel, this._config.ExchangeName);

                    // Публикуем
                    this._channel.BasicPublish(
                        this._config.ExchangeName,
                        routingKey,
                        basicProperties,
                        this._serializer.Serialize(item));
                }
                catch (Exception e)
                {
                    this._logger.Error("Ошибка публикации сообщения в очередь", e);
                    throw;
                }
            }
        }

        /// <summary>
        ///     Уведомить поставщика о том, что наблюдатель должен получать уведомления.
        /// </summary>
        /// <param name="mask">Маска</param>
        /// <param name="observer">Объект, который должен получать уведомления</param>
        /// <returns>Ссылка на подписку</returns>
        public IDisposable Subscribe(string mask, Func<QueueMessage, bool> observer)
        {
            var connection = this._observers.GetOrAdd(mask, s => this.Connect());
            var channel = this.GetСhannel(connection);

            // Инициализируем объекты RabbitMq
            this.InitializeRabbitMq(mask, channel, this._config.ExchangeName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, receive) =>
            {
                var message = this._serializer.Deserialize<QueueMessage>(receive.Body);
                var ask = observer(message);
                if (ask)
                {
                    channel.BasicAck(receive.DeliveryTag, true);
                }
                else
                {
                    channel.BasicNack(receive.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(GetQueueFullName(mask, this._config.ExchangeName), false, consumer);

            return new SubscriptionQueue(
                () =>
                {
                    channel.Close();
                    this.Disconnect(connection);
                });
        }

        /// <summary>
        ///     Уничтожает экземпляр класса <see cref="QueueRabbitMq" />
        /// </summary>
        ~QueueRabbitMq()
        {
            this.Dispose(false);
        }

        /// <summary>
        ///     Инициализировать объекты в очереди
        /// </summary>
        /// <param name="routingKey"> Путь </param>
        /// < param name="model"> Модель AMQP</param>
        /// <param name="exchangeName"> Наименование точки расширения</param>
        private void InitializeRabbitMq(string routingKey, IModel model, string exchangeName)
        {
            // Создадим точку обмена
            model.ExchangeDeclare(exchangeName, ExchangeType.Direct, this._config.ExchangeDurable);

            var queueFullName = GetQueueFullName(routingKey, exchangeName);

            // Создадим очередь
            model.QueueDeclare(
                queueFullName,
                this._config.QueueDurable,
                this._config.QueueExclusive,
                this._config.QueueAutoDelete,
                null);

            // Свяжем
            model.QueueBind(queueFullName, exchangeName, routingKey);
        }

        private static string GetQueueFullName(string routingKey, string exchangeName)
        {
            return $"{exchangeName}:{routingKey}";
        }

        /// <summary>
        ///     Подключиться к очереди
        /// </summary>
        private IConnection Connect()
        {
            var connection = this.TryToConnect(this._factory, this._config);

            // Подписываемся на события
            this.SubscribeToEvents(connection);

            return connection;
        }

        /// <summary>
        ///     Получить модель
        /// </summary>
        /// <param name="connection">Соединение</param>
        /// <returns>Модель</returns>
        private IModel GetСhannel(IConnection connection)
        {
            var model = connection.CreateModel();

            model.BasicQos(0, this._config.PrefetchCount, true);

            return model;
        }

        /// <summary>
        ///     Отключиться от очереди
        /// </summary>
        /// <param name="connection">Соединение</param>
        private void Disconnect(IConnection connection)
        {
            // Отписываемся от событий
            this.UnsubscribeToEvents(connection);
            connection?.Dispose();
        }

        /// <summary>
        ///     Выполнить определяемые приложением задачи, связанные с высвобождением или сбросом неуправляемых ресурсов
        /// </summary>
        /// <param name="disposing">Освобождать</param>
        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._channel?.Dispose();
                this.Disconnect(this._connection);

                foreach (var connection in this._observers.Values)
                {
                    connection.Close();
                }

                this._observersLock.Dispose();
            }

            this._disposed = true;
        }

        /// <summary>
        ///     Обработать событие полученное при возникновении ошибки в RabbitMQ
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="args">Аргументы события</param>
        private void OnCallbackException(object sender, CallbackExceptionEventArgs args)
        {
            this._logger.Error("Ошибка в RabbitMQ", args.Exception);
        }

        /// <summary>
        ///     Обработчик блокировки соединения
        /// </summary>
        /// <param name="sender">Соединение</param>
        /// <param name="args">Аргументы</param>
        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs args)
        {
            this._logger.Info($"Соединение с RabbitMQ заблокировано. Причина: '{args.Reason}'");
        }

        /// <summary>
        ///     Обработать событие разрыва соединения
        /// </summary>
        /// <param name="sender">Соединение</param>
        /// <param name="reason">Причина</param>
        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (this._disposed)
            {
                return;
            }

            this._logger.Info("Разрыв соединения с RabbitMQ.");
        }

        /// <summary>
        ///     обработчик события разблокировки соединения
        /// </summary>
        /// <param name="sender">Соединение</param>
        /// <param name="eventArgs">Аргументы</param>
        private void OnConnectionUnblocked(object sender, EventArgs eventArgs)
        {
            this._logger.Info("Соединение разблокировано");
        }

        /// <summary>
        ///     Подписаться на все необходимые события
        /// </summary>
        /// <param name="connection">Соединение</param>
        private void SubscribeToEvents(IConnection connection)
        {
            connection.CallbackException += this.OnCallbackException;
            connection.ConnectionShutdown += this.OnConnectionShutdown;
            connection.ConnectionBlocked += this.OnConnectionBlocked;
            connection.ConnectionUnblocked += this.OnConnectionUnblocked;
        }

        /// <summary>
        ///     Установить соединение
        /// </summary>
        /// <param name="factory">Фабрика соединений</param>
        /// <param name="config">Конфигурация</param>
        /// <returns>Установленное соединение</returns>
        private IConnection TryToConnect(ConnectionFactory factory, IConfigurationRabbitMq config)
        {
            if (this._disposed)
            {
                return null;
            }

            factory.HostName = string.IsNullOrEmpty(config.HostName) ? "localhost" : config.HostName;
            factory.UserName = string.IsNullOrEmpty(config.UserName) ? "guest" : config.UserName;
            factory.Password = string.IsNullOrEmpty(config.Password) ? "guest" : config.Password;

            this._logger.Debug($"Начинаем устанавливать соединение к {config.HostName}");

            IConnection connection = null;
            do
            {
                try
                {
                    connection = factory.CreateConnection();
                }
                catch (Exception e)
                {
                    this._logger.Error($"Ошибка соединения к {config.HostName}", e);

                    // Соединение не удалось делаем паузу
                    Thread.Sleep(this._config.ReconnectInterval);
                }
            }
            while (connection == null || !connection.IsOpen);

            this._logger.Debug($"Соединение к {config.HostName} установлено.");

            return connection;
        }

        /// <summary>
        ///     Отписаться от событий
        /// </summary>
        /// <param name="connection">Соединение</param>
        private void UnsubscribeToEvents(IConnection connection)
        {
            connection.CallbackException -= this.OnCallbackException;
            connection.ConnectionShutdown -= this.OnConnectionShutdown;
            connection.ConnectionBlocked -= this.OnConnectionBlocked;
            connection.ConnectionUnblocked -= this.OnConnectionUnblocked;
        }

        /// <summary>
        ///     Подписка
        /// </summary>
        private class SubscriptionQueue : IDisposable
        {
            /// <summary>
            ///     Действие отписаться
            /// </summary>
            private readonly Action _unsubscribe;

            /// <summary>
            ///     Флаг ресурсы освобождены
            /// </summary>
            private bool _disposed;

            /// <summary>
            ///     Инициализирует новый экземпляр класса <see cref="SubscriptionQueue" />
            /// </summary>
            /// <param name="unsubscribe">Действие отписывания</param>
            public SubscriptionQueue(Action unsubscribe)
            {
                this._unsubscribe = unsubscribe;
            }

            /// <summary>
            ///     Освободить неуправляемые ресурсы
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
            }

            /// <summary>
            ///     Уничтожает экземпляр класса <see cref="SubscriptionQueue" />
            /// </summary>
            ~SubscriptionQueue()
            {
                this.Dispose(false);
            }

            /// <summary>
            ///     Освободить неуправляемые ресурсы
            /// </summary>
            /// <param name="disposing">Освобождать</param>
            private void Dispose(bool disposing)
            {
                if (this._disposed)
                {
                    return;
                }

                if (disposing)
                {
                    var action = this._unsubscribe;
                    action?.Invoke();
                }

                this._disposed = true;
            }
        }
    }
}