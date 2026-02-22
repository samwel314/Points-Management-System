using Microsoft.AspNetCore.Identity;

namespace Robi_App.Identity_Error_Describer
{
    public class ArabicIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
       => new IdentityError
       {
           Code = nameof(DuplicateUserName),
           Description = $"رقم المستخدم '{userName}' مستخدم بالفعل"
       };

        public override IdentityError PasswordTooShort(int length)
            => new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"كلمة المرور يجب ألا تقل عن {length} أحرف"
            };

        public override IdentityError PasswordRequiresDigit()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "كلمة المرور يجب أن تحتوي على رقم واحد على الأقل"
            };

        public override IdentityError InvalidUserName(string userName)
            => new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = " رقم الهاتف غير صحيح او كلمة السر غير صحيحة   "
            };

       
    }
}
