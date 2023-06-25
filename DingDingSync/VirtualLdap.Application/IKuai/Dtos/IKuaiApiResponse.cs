namespace VirtualLdap.Application.IKuai.Dtos
{

    public class IKuaiApiResponseList<T>
    {
        public int ErrorCode { get; set; }


        public string ErrorMsg { get; set; }


        public List<T> Data { get; set; }
    }

    public class IKuaiApiResponse<T>
    {
        public int ErrorCode { get; set; }


        public string ErrorMsg { get; set; }


        public T Data { get; set; }
    }
}
