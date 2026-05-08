namespace HridhayConnect_API.Infra
{
    public class PagedResult<T>
    {
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public IEnumerable<T> Data { get; set; }

    }
}
