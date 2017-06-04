using System;

namespace MarketParserNet.Test
{
    /// <summary>
    ///     �������� ������ ��������
    /// </summary>
    [Serializable]
    public class MockObject : IEquatable<MockObject>
    {
        /// <summary>
        ///     ���������
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     ���������, ����� �� ������� ������ ������� ������� ���� �� ����.
        /// </summary>
        /// <returns>
        ///     true, ���� ������� ������ ����� ��������� <paramref name="other" />, � ��������� �����堗 false.
        /// </returns>
        /// <param name="other">������, ������� ��������� �������� � ������ ��������.</param>
        public bool Equals(MockObject other)
        {
            return other != null && this.Message.Equals(other.Message);
        }

        /// <summary>
        ///     ����������, ����� �� �������� ������ �������� �������.
        /// </summary>
        /// <returns>
        ///     �������� true, ���� ��������� ������ ����� �������� �������; � ��������� ������ � �������� false.
        /// </returns>
        /// <param name="obj">������, ������� ��������� �������� � ������� ��������. </param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as MockObject);
        }

        /// <summary>
        /// ������ ���-�������� �� ���������. 
        /// </summary>
        /// <returns>
        /// ���-��� ��� �������� �������.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Message?.GetHashCode() ?? 0;
        }
    }
}