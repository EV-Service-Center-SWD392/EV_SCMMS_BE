using System.ComponentModel.DataAnnotations;

public class VehicleQueryDto
{
  public int Page { get; set; } = 1;

  public int PageSize { get; set; } = 10;

  public string? Status { get; set; }  // Filter ACTIVE/INACTIVE

  public int? Year { get; set; }

  public DateTime? FromDate { get; set; }  // Filter CreatedAt >= FromDate

  public DateTime? ToDate { get; set; }  // Filter CreatedAt <= ToDate
}
