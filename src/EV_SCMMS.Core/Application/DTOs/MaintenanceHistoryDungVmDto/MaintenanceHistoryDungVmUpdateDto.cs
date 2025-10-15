using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.DTOs.MaintenanceHistoryDungVmDto
{
    public class MaintenanceHistoryDungVmUpdateDto
    {
        public int OrderId { get; set; }

        [Required]
        public DateTime? CompletedDate { get; set; }

        [StringLength(1000)]
        public string? Summary { get; set; }
    }
}
