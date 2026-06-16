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
    userLogin: string;
    userFirstName: string;
    userLastName: string;
    isApproved: boolean;
    approvedAt: string;
}

export interface AchievementApprovalSummary {
    approved: number;
    denied: number;
    currentUserVote: boolean | null;
}