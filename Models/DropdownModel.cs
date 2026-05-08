namespace HridhayConnect_API.Models
{
    public class Dropdownname
    {
        public long Id { get; set; }
        public string? Name { get; set; }

    }
    public class Lov_Master
    {
        public string? Code { get; set; }
        public string? Name { get; set; }

    }

    public class ItemDropdown
    {
        public long Id { get; set; }
        public string? Item_Name { get; set; }
        public decimal? Rate { get; set; }

    }
}
