export interface Achievement {
    achievementId: string;
    userId: string;
    title: string;
    description?: string;
    createdAt: string;
    updatedAt: string;
}

export interface AchievementApprove {
    achievementApproveId: string;
    achievementId: string;
    userId: string;
    isApproved: boolean;
    approvedAt: string;
}