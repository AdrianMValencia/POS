using POS.Application.Commons.Bases.Request;
using POS.Application.Commons.Bases.Response;
using POS.Application.Commons.Select.Response;
using POS.Application.Dtos.User.Request;
using POS.Application.Dtos.User.Response;

namespace POS.Application.Interfaces
{
    public interface IUserApplication
    {
        Task<BaseResponse<IEnumerable<UserResponseDto>>> ListUsers(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<SelectResponse>>> ListSelectUsers();
        Task<BaseResponse<UserByIdResponseDto>> UserById(int userId);
        Task<BaseResponse<bool>> RegisterUser(UserRequestDto requestDto);
    }
}