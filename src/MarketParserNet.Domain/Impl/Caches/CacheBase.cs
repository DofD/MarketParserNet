using System;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    public abstract class CacheBase<T> : ICache<string, T>, IDisposable
    {
        /// <summary>
        ///     ��������� ��� �����
        /// </summary>
        private readonly IHashGenerator _hashGenerator;

        protected CacheBase(IHashGenerator hashGenerator)
        {
            this._hashGenerator = hashGenerator;
        }

        /// <summary>
        ///     ���������� ������� ����
        /// </summary>
        /// <param name="id">����</param>
        /// <returns>������� ����</returns>
        public abstract T Get(string id);

        /// <summary>
        ///     �������� �������
        /// </summary>
        /// <param name="element">�������</param>
        /// <param name="ttl">����� ����� � ��������</param>
        /// <returns>����</returns>
        public virtual string Add(T element, int? ttl = null)
        {
            var key = this.GetMd5(element);

            this.Add(key, element, ttl);
            return key;
        }

        /// <summary>
        ///     �������� ����� �������
        /// </summary>
        /// <param name="key">����</param>
        /// <param name="element">�������</param>
        /// <param name="ttl">����� ����� � ��������</param>
        public abstract void Add(string key, T element, int? ttl = null);

        /// <summary>
        ///     �������� ������� ����
        /// </summary>
        /// <param name="id">����</param>
        /// <returns>����� ��������</returns>
        public abstract bool Reset(string id);

        /// <summary>
        ///     �������� ���
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     ��������� ������������ ����������� ������, ��������� � ���������, �������������� ��� ������� �������������
        ///     ��������.
        /// </summary>
        public abstract void Dispose();

        protected string GetMd5(T element)
        {
            return this._hashGenerator.GetMd5(element);
        }
    }
}