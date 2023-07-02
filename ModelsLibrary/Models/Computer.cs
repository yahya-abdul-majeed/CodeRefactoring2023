using ModelsLibrary.Models.DTO;
using ModelsLibrary.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelsLibrary.Models
{
    public class Computer
    {
        public int ComputerId { get;set; }
        public string ComputerName { get;set;}
        public string Description { get;set; }
        public bool IsPositioned { get;set; }
        public GridType GridType { get; set; }
        public int? PositionOnGrid { get; set; } = null;
        [ForeignKey("Lab")]
        public int? LabId { get;set; }
        public Lab? Lab { get; set; } //navigation property

        public bool IsValid()
        {
            return ComputerName == string.Empty;
        }
        public void UpdatePositionInfo(PositionUpdateDTO dto)
        {
            IsPositioned = dto.IsPositioned;
            PositionOnGrid = dto.PositionOnGrid;
            LabId = dto.LabId;
        }

    }
}
