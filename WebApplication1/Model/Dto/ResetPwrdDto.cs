namespace WebApplication1.Model.Dto
{
    public record ResetPwrdDto
    {
        public String Email { get; set; }
        public String EmailToken{ get; set; }
        public String NewPassword { get; set; }
        public String ConfirmPassword { get; set; }
    }
}
