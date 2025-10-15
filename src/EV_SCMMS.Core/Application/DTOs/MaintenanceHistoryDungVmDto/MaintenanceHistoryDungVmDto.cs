using EV_SCMMS.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.DTOs.MaintenanceHistoryDungVmDto
{
    public class MaintenanceHistoryDungVmDto
    {
        public Guid? MaintenanceHistoryDungVmid { get; set; }
        [Required]
        public Guid VehicleId { get; set; }

        public int OrderId { get; set; }

        [Required]
        public DateTime? CompletedDate { get; set; }

        [StringLength(1000)]
        public string? Summary { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsActive { get; set; }

    }
}
