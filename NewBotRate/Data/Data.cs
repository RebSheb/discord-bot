using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NewBotRate.Database;

namespace NewBotRate.Data
{
    public static class Data
    {
        public static List<Feedback> GetFeedbacks(int Amount = 10, bool GetRead = false)
        {
            List<Feedback> RetList = null;
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Feedbacks.Where(x => x.Read == (GetRead ? 1 : 0)).Count() < 1)
                {
                    return RetList;
                }
                return DbContext.Feedbacks.Where(x => x.Read == (GetRead ? 1 :  0)).Take(Amount).ToList();
            }
        }

        public static async void AddFeedback(ulong GID, ulong UID, string MSG)
        {
            using (var DbContext = new SqliteDbContext())
            {
                await DbContext.Feedbacks.AddAsync(new Feedback
                {
                    GuildID = GID,
                    UserID = UID,
                    Message = MSG,
                    Read = 0,
                });


                await DbContext.SaveChangesAsync();
            }
        }

        public static async void DeleteFeedback(int FBackID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Feedbacks.Where(x => x.FeedbackID == FBackID).Count() == 1)
                {
                    //await DbContext.Feedbacks.Where(x => x.FeedbackID == FBackID)
                    DbContext.Entry(new Feedback() { FeedbackID = FBackID }).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    await DbContext.SaveChangesAsync();
                }
            }

        }

        public static async void EditFeedbackRead(int FBackID, bool readVal)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if(DbContext.Feedbacks.Where(x=> x.FeedbackID == FBackID).Count() == 1)
                {
                    Feedback Current = DbContext.Feedbacks.Where(x => x.FeedbackID == FBackID).FirstOrDefault();
                    Current.Read = (readVal == true) ? 1 : 0;
                    DbContext.Feedbacks.Update(Current);
                }
                await DbContext.SaveChangesAsync();
            }
        }



        public static string AddTag(ulong GID, ulong UID, string tagName, string msg)
        {
            using (var DbContext = new SqliteDbContext()) 
            {
                if (DbContext.Tags.Where(x => x.GuildID == GID && x.TagName == tagName).Count() >= 1)
                {
                    return "This tag has already been taken!";
                }

                DbContext.Tags.Add(new Tag
                {
                    GuildID = GID,
                    UserID = UID,
                    TagName = tagName,
                    TagMsg = msg,
                });

                DbContext.SaveChanges();
            }
            return $"Successfully added tag {tagName}";
        }         
        

        public static List<Tag> GetTags(ulong GID, ulong UID, int paginate = 0)
        {
            List<Tag> retTags = null;
            using (var DbContext = new SqliteDbContext())
            {
                if(DbContext.Tags.Where(x=> x.GuildID == GID && x.UserID == UID).Skip(24 * paginate).Count() >= 1)
                {
                    return DbContext.Tags.Where(x => x.GuildID == GID && x.UserID == UID).Skip(24 * paginate).Take(24).ToList();
                }
            }

            return retTags;
        }

        public static int GetTagCountUser(ulong GID, ulong UID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                return DbContext.Tags.Where(x => x.GuildID == GID && x.UserID == UID).Count();
            }
        }
    }
}
