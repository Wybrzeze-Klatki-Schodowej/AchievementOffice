export interface AchievementVerificationRequest {
    id: string;
    achievementId: string;
    achievementTitle: string;
    requesterUserId: string;
    requesterLogin: string;
    targetUserId: string;
    targetUserLogin: string;
    status: string;
    createdAt: string;
    respondedAt?: string | null;
}

export interface UserReviewer {
    userId: string;
    login: string;
    firstName: string;
    lastName: string;
}