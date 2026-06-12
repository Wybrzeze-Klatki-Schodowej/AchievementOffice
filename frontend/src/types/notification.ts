export interface Notification {
    id: string;
    type: string;
    title: string;
    message: string;
    isRead: boolean;
    createdAt: string;
    verificationRequestId?: string | null;
    achievementId?: string | null;
    achievementOwnerId?: string | null;
    achievementTitle?: string | null;
    achievementDescription?: string | null;
    achievementOwnerLogin?: string | null;
}