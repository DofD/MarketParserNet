using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

using ConnectionFactory = RabbitMQ.Client.ConnectionFactory;
using IBasicConsumer = RabbitMQ.Client.IBasicConsumer;
using IConnection = RabbitMQ.Client.IConnection;
using IModelExensions = RabbitMQ.Client.IModelExensions;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Адаптер очереди RabbitMQ
    /// </summary>
    public class QueueRabbitMq : IQueue, IDisposable
    {
        /// <summary>
        ///     Объект блокировки потоков авто извлечения
        /// </summary>
        private readonly object _autoDequeueThreadsLock = new object();

        /// <summary>
        ///     Конфигурация очереди
        /// </summary>
        private readonly IConfigurationRabbitMq _config;

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
        private readonly ConcurrentDictionary<string, List<Func<QueueMessage, bool>>> _observers =
            new ConcurrentDictionary<string, List<Func<QueueMessage, bool>>>();

        /// <summary>
        ///     Блокировка подписчиков
        /// </summary>
        private readonly ReaderWriterLockSlim _observersLock = new ReaderWriterLockSlim();

        /// <summary>
        ///     Соединение
        /// </summary>
        private IConnection _connection;

        /// <summary>
        ///     Флаг очистки ресурсов
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Модель AMQP
        /// </summary>
        private IModel _model;

        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="QueueRabbitMq" />
        /// </summary>
        /// <param name="factory">Фабрика подключения</param>
        /// <param name="configManager">Менеджер конфигурации</param>
        /// <param name="logger">Логировщик</param>
        public QueueRabbitMq(
            ConnectionFactory factory,
            IConfigManager<IConfigurationRabbitMq> configManager,
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

            this._config = configManager.GetConfiguration();

            this.Connect();
        }

        /// <summary>
        ///     Выполнить определяемые приложением задачи, связанные с высвобождением или сбросом неуправляемых ресурсов
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Уничтожает экземпляр класса <see cref="QueueRabbitMq" />
        /// </summary>
        ~QueueRabbitMq()
        {
            this.Dispose(false);
        }

        /// <summary>
        ///     Вернуть актуальный элемент очереди
        /// </summary>
        /// <param name="routingKey">Маска</param>
        /// <returns>Элемент очереди</returns>
        public QueueMessage Dequeue(string routingKey)
        {
            var queueFullName = routingKey;

            try
            {
                var receive = this._model.BasicGet(queueFullName, false);
                if (receive == null)
                {
                    return null;
                }

                var message = Deserialize(receive.Body);

                this._model.BasicAck(receive.DeliveryTag, false);

                return message;
            }
            catch (Exception e)
            {
this._logger.Error("Ошибка получения элемента из очереди", e);

                return null;
            }
        }

        /// <summary>
        ///     Добавить новый элемент в очередь
        /// </summary>
        /// <param name="item">Добавляемый элемент</param>
        /// <param name="routingKey">Маска</param>
        public void Enqueue(QueueMessage item, string routingKey)
        {
            //// Создадим базовые параметры
            //var basicProperties = this._model.CreateBasicProperties();

            //// Говорим что сообщение постоянное
            //basicProperties.Persistent = true;

            //var configMessage = this.GetConfigurationMessageRabbitMq(item.GetType());

            //var configExchange = configMessage.ExchangeConfig ?? this._configManager.GetDefaultExchangeConfig();
            //var configQueue = configMessage.QueueConfig ?? this._configManager.GetDefaultQueueConfig();

            //item.TracingOperation.AddPoint(this._checkPointManager.CreatePoint(ENQUEUE + SEND, routingKey));
            //item.TracingOperation.AddRoute(routingKey);

            //// Создаем прокси объект
            //var proxy = this._proxyObjectFactory.CreateProxy(item);
            //proxy.Self = Encoding.UTF8.GetString(this._serializer.Serialize(item));

            //// Обрабатываем информационный блок
            //proxy.Info.Header = item.TracingOperation.GetHeader();
            //proxy.Info.Route = item.TracingOperation.GetRoute();
            //proxy.Info.CheckPoints = item.TracingOperation.GetCheckPoints();

            //var exchangeName = this.GetExchangeName(configExchange.Name, item);

            //// блокируем модель иначе может вызваться исключение при много поточности
            //lock (this._modelLock)
            //{
            //    try
            //    {
            //        // Инициализируем объекты RabbitMq
            //        InitializeRabbitMq(configExchange, configQueue, routingKey, this._model, exchangeName);

            //        // Публикуем
            //        this._model.BasicPublish(
            //            exchangeName,
            //            routingKey,
            //            configMessage.Mandatory,
            //            configMessage.Immediate,
            //            basicProperties,
            //            this._serializer.Serialize(proxy));
            //    }
            //    catch (Exception e)
            //    {
            //        this._errorHandler.HandleError(this, e);
            //    }
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Уведомить поставщика о том, что наблюдатель должен получать уведомления.
        /// </summary>
        /// <param name="mask">Маска</param>
        /// <param name="observer">Объект, который должен получать уведомления</param>
        /// <returns>Ссылка на подписку</returns>
        public IDisposable Subscribe(string mask, Func<QueueMessage, bool> observer)
        {
            if (!this._observers.ContainsKey(mask))
            {
                this._observers[mask] = new List<Func<QueueMessage, bool>>();
            }

            // Добавляем в список подписчиков
            if (!this._observers[mask].Contains(observer))
            {
                this._observersLock.EnterWriteLock();
                try
                {
                    this._observers[mask].Add(observer);
                }
                finally
                {
                    this._observersLock.ExitWriteLock();
                }
            }

            // Включаем автоматическое получение от RabbitMQ
            this.TurnOnAutoDequeue(mask);

            return new SubscriptionQueue(() => this.Unsubscribe(mask, observer));
        }

        /// <summary>
        ///     Получить полное имя очереди
        /// </summary>
        /// <param name="queueConfig">Конфигурация очереди</param>
        /// <param name="routingKey">Маска</param>
        /// <param name="exchangeName">Имя точки расширения</param>
        /// <returns>Полное имя очереди</returns>
        //private static string GetQueueFullName(IQueueConfig queueConfig, string routingKey, string exchangeName = null)
        //{
        //    var name = exchangeName ?? queueConfig.Name;
        //    return name + ":" + routingKey;
        //}

        /// <summary>
        ///     Инициализировать объекты в очереди
        /// </summary>
        /// <param name="exchangeConfig">Настройки точки обмена</param>
        /// <param name="queueConfig">Настройки очереди</param>
        /// <param name="routingKey">Путь</param>
        /// <param name="model">Модель AMQP</param>
        /// <param name="exchangeName">Наименование точки расширения</param>
        //private static void InitializeRabbitMq(
        //    IExchangeConfig exchangeConfig,
        //    IQueueConfig queueConfig,
        //    string routingKey,
        //    IModel model,
        //    string exchangeName = null)
        //{
        //    exchangeName = exchangeName ?? exchangeConfig.Name;

        //    // Создадим точку обмена
        //    model.ExchangeDeclare(exchangeName, exchangeConfig.Type, exchangeConfig.Durable);

        //    var queueFullName = GetQueueFullName(queueConfig, routingKey, exchangeName);

        //    // Создадим очередь
        //    model.QueueDeclare(queueFullName, queueConfig.Durable, queueConfig.Exclusive, queueConfig.AutoDelete, null);

        //    // Свяжем
        //    model.QueueBind(queueFullName, exchangeName, routingKey);
        //}

        /// <summary>
        ///     Авто получение сообщений
        /// </summary>
        /// <param name="routingKey">Маска</param>
        private void AutoDequeue(string routingKey)
        {
            throw new NotImplementedException();

            //// Создается параллельное соединение
            //var configMessage = this.GetConfigurationMessageRabbitMq(typeof(QueueMessage));
            //var configQueue = configMessage.QueueConfig ?? this._configManager.GetDefaultQueueConfig();
            //var queueFullName = GetQueueFullName(configQueue, routingKey);
            //var arg = this._configManager.GetConsumerArgument(routingKey);

            //var lastGetTime = DateTime.MinValue;
            //EventingBasicConsumer consumer = null;
            //var consumerTag = Guid.NewGuid().ToString();

            //// сохраняем предыдущую модель так как при переподключение будет создана новая
            //var model = this._model;

            //// блокируем модель иначе может вызваться исключение при много поточности
            //lock (this._modelLock)
            //{
            //    // Создадим очередь
            //    model.QueueDeclare(
            //        queueFullName,
            //        configQueue.Durable,
            //        configQueue.Exclusive,
            //        configQueue.AutoDelete,
            //        null);
            //}

            //// Флаг отправки пина
            //var sendPing = false;

            //// Флаг обработки
            //var isRun = false;

            //// Таймер нужен, так как иногда потребитель зависает
            //this._consumerRecreateTimers[routingKey] = new WrapTimer(
            //    timer =>
            //    {
            //        if (isRun)
            //        {
            //            return;
            //        }

            //        if (DateTime.UtcNow - lastGetTime < TimeSpan.FromMilliseconds(this._config.ConsumerRecreateTime)
            //            && model == this._model)
            //        {
            //            return; // Все нормально потребитель не завис
            //        }

            //        isRun = true;

            //        if (model != this._model)
            //        {
            //            consumer = null;
            //        }

            //        if (consumer != null)
            //        {
            //            // Пинг был отправлен, а ответа так и нет
            //            if (sendPing)
            //            {
            //                // Пересоздаем    
            //                consumer.OnCancel();
            //                lock (this._modelLock)
            //                {
            //                    this._model.BasicCancel(consumerTag);
            //                }

            //                sendPing = false;
            //                this._logger.Debug(this, string.Format(Resources.ConsumerRecreateByRoute, routingKey));
            //            }
            //            else
            //            {
            //                // Отправим пинг
            //                this.Enqueue(
            //                    new PingMessage { SendTime = DateTime.UtcNow, CryptoProviderName = configQueue.Name },
            //                    routingKey);

            //                sendPing = true;

            //                this._logger.Debug(this, string.Format("Отправили пинг на [{0}]", routingKey));

            //                isRun = false;
            //                return;
            //            }
            //        }

            //        lock (this._modelLock) // блокируем модель иначе может вызваться исключение
            //        {
            //            consumer = new EventingBasicConsumer(this._model);

            //            try
            //            {
            //                IModelExensions.BasicConsume(
            //                    this._model,
            //                    queueFullName,
            //                    (bool)false,
            //                    consumerTag,
            //                    (IDictionary<string, object>)arg,
            //                    (IBasicConsumer)consumer);
            //            }
            //            catch (OperationInterruptedException e)
            //            {
            //                consumer = null;

            //                this._errorHandler.HandleError(this, e);
            //                return;
            //            }
            //        }

            //        this._logger.Debug(this, string.Format(Resources.SubscribersToRoute, routingKey));

            //        try
            //        {
            //            isRun = false; // так как тут получение из очереди идет через ожидание

            //            consumer.Received += (sender, args) =>
            //            {
            //                if (this._config.AsParallel)
            //                {
            //                    try
            //                    {
            //                        ThreadPool.QueueUserWorkItem(
            //                            state =>
            //                            {
            //                                sendPing = false; // Сбрасываем отправку пинг пакета
            //                                lastGetTime = this.HandleMessage(routingKey, args);
            //                            });
            //                    }
            //                    catch (Exception e)
            //                    {
            //                        this._errorHandler.HandleError(this, e);
            //                        this._model.BasicNack(args.DeliveryTag, false, true);
            //                    }
            //                }
            //                else
            //                {
            //                    sendPing = false; // Сбрасываем отправку пинг пакета
            //                    lastGetTime = this.HandleMessage(routingKey, args);
            //                }
            //            };
            //        }
            //        catch (Exception e)
            //        {
            //            this._errorHandler.HandleError(this, e);
            //        }
            //    },
            //    this._config.ConsumerRecreateTime);
        }

        /// <summary>
        ///     Подключиться к очереди
        /// </summary>
        private void Connect()
        {
            this._connection = this.TryToConnect(this._factory, this._config);
            this._model = this._connection.CreateModel();

            this._model.BasicQos(0, this._config.PrefetchCount, true);

            // Подписываемся на события
            this.SubscribeToEvents(this._connection);
        }

        /// <summary>
        ///     Десериализовать
        /// </summary>
        /// <param name="body">Сообщение из очереди</param>
        /// <returns>Полученное сообщение</returns>
        private static QueueMessage Deserialize(byte[] body)
        {
            return body == null ? null : JsonConvert.DeserializeObject<QueueMessage>(Encoding.UTF8.GetString(body));
        }

        /// <summary>
        ///     Отключиться от очереди
        /// </summary>
        private void Disconnect()
        {
            // Отписываемся от событий
            this.UnsubscribeToEvents(this._connection);

            this._model?.Dispose();

            this._connection?.Dispose();
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
                this.Disconnect();

                //foreach (var autoDequeueThread in this._autoDequeueThreads)
                //{
                //    this.ShutDownAutoDequeue(autoDequeueThread.Key);
                //}

                //// Завершаем таймеры
                //foreach (var consumerRecreateTimer in this._consumerRecreateTimers)
                //{
                //    consumerRecreateTimer.Value.Dispose();
                //}

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
        ///     Выключить автоматическое получение сообщений
        /// </summary>
        /// <param name="mask">Маска</param>
        private void ShutDownAutoDequeue(string mask)
        {
            throw new NotImplementedException();

            //if (!this._autoDequeueThreads.ContainsKey(mask))
            //{
            //    return;
            //}

            //lock (this._autoDequeueThreadsLock)
            //{
            //    if (this._autoDequeueThreads[mask].IsAlive)
            //    {
            //        this._autoDequeueThreads[mask].Abort();
            //    }

            //    Thread thread;
            //    this._autoDequeueThreads.TryRemove(mask, out thread);
            //}
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
        ///     Включить автоматическое получение из очереди
        /// </summary>
        /// <param name="mask">Маска</param>
        private void TurnOnAutoDequeue(string mask)
        {
            throw new NotImplementedException();

            //if (!this._autoDequeueThreads.ContainsKey(mask))
            //{
            //    this._autoDequeueThreads[mask] = new Thread(() => this.AutoDequeue(mask)) { Name = mask };
            //}

            //lock (this._autoDequeueThreadsLock)
            //{
            //    if (!this._autoDequeueThreads[mask].IsAlive)
            //    {
            //        this._autoDequeueThreads[mask].Start();
            //    }
            //}
        }

        /// <summary>
        ///     Отписаться
        /// </summary>
        /// <param name="mask">Маска</param>
        /// <param name="observer">Подписчик</param>
        private void Unsubscribe(string mask, Func<QueueMessage, bool> observer)
        {
            this._logger.Debug($" --> Unsubscribe {mask}");
            if (!this._observers.ContainsKey(mask))
            {
                this._logger.Debug(" <-- Unsubscribe NoMask");
                return;
            }

            this._observersLock.EnterWriteLock();
            try
            {
                this._observers[mask].RemoveAll(s => s == observer);
            }
            finally
            {
                this._observersLock.ExitWriteLock();
            }

            this.ShutDownAutoDequeue(mask);

            this._logger.Debug($" <-- Unsubscribe {mask}");
        }

        /// <summary>
        ///     Отписаться от событий
        /// </summary>
        /// <param name="connection">Соединение</param>
        private void UnsubscribeToEvents(IConnection connection)
        {
            connection.CallbackException -= this.OnCallbackException;
            connection.ConnectionShutdown -= this.OnConnectionShutdown;
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