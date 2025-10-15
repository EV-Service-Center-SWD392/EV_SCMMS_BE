using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.DTOs.VehicleConditionDungVm
{
    public class VehicleConditionDungVmDto
    {
        public Guid? VehicleConditionDungVmid { get; set; }

        public Guid VehicleId { get; set; }

        public DateTime? LastMaintenance { get; set; }

        public string? Condition { get; set; }
        
        [StringLength(20)]
        public string? Status { get; set; }
        
        [Range(0, 100)]
        public int? BatteryHealth { get; set; }

        public int? TirePressure { get; set; }

        [StringLength(255)]
        public string? BrakeStatus { get; set; }

        [StringLength(255)]
        public string? BodyStatus { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

    }
}
