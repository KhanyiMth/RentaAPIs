using Microsoft.ML.Data;

namespace RentaAPIs.ML.Models
{
    public class ModelOutput
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
