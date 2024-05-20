using Domain.Bislerium.Models;
using Domain.Bislerium.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface ICommentService
    {
        //for comments
        Task<IEnumerable<Comment>> GetCommentsForPostAsync(int postId);
        //for geting all commetnsasync

        Task<IEnumerable<Comment>> GetAllCommentsAsync();

        //for adding comments
        Task<GeneralServiceResponseDto> AddCommentAsync(int postId, string userId, CommentDto commentDto);

        //for editcomments
        Task<GeneralServiceResponseDto> EditCommentAsync(int commentId, int blogId, string userId, CommentDto commentDto);

//for delete comments
        Task<bool> DeleteCommentAsync(int commentId, string userId);

        //for getting comment history

        Task<IEnumerable<CommentHistory>> GetCommentHistoryAsync(int commentId);

        


    }
}
