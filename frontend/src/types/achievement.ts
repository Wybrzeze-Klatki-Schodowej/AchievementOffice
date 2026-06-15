export interface Achievement {
    achievementId: string;
    userId: string;
    title: string;
    description?: string;
    createdAt: string;
    updatedAt: string;
    visibilityId: number; // 1=Public, 2=Private, 3=Group
    groupIds: string[];
}

export interface AchievementApprove {
    achievementApproveId: string;
    achievementId: string;
    userId: string;
    isApproved: boolean;
    approvedAt: string;
}

export interface AchievementApprovalSummary {
    approved: number;
    denied: number;
    currentUserVote: boolean | null;
}