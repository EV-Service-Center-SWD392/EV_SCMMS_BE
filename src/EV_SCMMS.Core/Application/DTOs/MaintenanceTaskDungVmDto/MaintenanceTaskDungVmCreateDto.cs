using EV_SCMMS.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.DTOs.MaintenanceTaskDungVmDto
{
    public class MaintenanceTaskDungVmCreateDto
    {
        [Required]
        public int OrderServiceId { get; set; }

        public Guid? TechnicianId { get; set; }

        [Required]
        [StringLength(255)]
        public string Task { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(20)]
        [EnumDataType(typeof(TaskEnum))]
        public string Status { get; set; } = "Pending";
    }
}
