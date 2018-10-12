using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface INotificationRepository
    {
        bool VerifyExistingOpenNotification(int packageId);
        void RequestPackageClosure(int packageId, int petitionerId, out bool wasClosed);
        bool CheckPendingAnswer(int notificationAnswerId, int userId);
        void AnswerClosureNotification(int notificationAnswerId, int userId, bool isAffirmative, out bool wasClosed);
        IEnumerable<NotificationAnswer> GetPendingNotifications(int userId);
        int GetPendingNotificationsCount(int userId);
        void RefreshNotificationsForUpdatedUser(int userId);
    }
}
