﻿namespace ModelsLibrary.Models.DTO
{
    public class ComputerCreationDTO
    {
        public string ComputerName { get; set; }
        public string Description { get; set; }
        public bool IsPositioned { get; set; }
        public int? LabId { get; set; }
    }
}
