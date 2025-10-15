using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.DTOs.VehicleConditionDungVm
{
    public class VehicleConditionDungVmUpdateDto
    {
        public DateTime? LastMaintenance { get; set; }

        [StringLength(255)]
        public string? Condition { get; set; }

        [StringLength(255)]
        public string? Status { get; set; }

        [Range(0, 100)]
        public int? BatteryHealth { get; set; }

        public int? TirePressure { get; set; }

        [StringLength(50)]
        public string? BrakeStatus { get; set; }

        [StringLength(50)]
        public string? BodyStatus { get; set; }
    }
}
