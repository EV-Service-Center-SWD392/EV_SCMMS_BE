using EV_SCMMS.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.DTOs.MaintenanceTaskDungVmDto
{
    public class MaintenanceTaskDungVmUpdateDto
    {
        public Guid? TechnicianId { get; set; }

        [StringLength(255)]
        public string Task { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(50)]
        [EnumDataType(typeof(TaskEnum))]
        public string Status { get; set; } 
    }
}
