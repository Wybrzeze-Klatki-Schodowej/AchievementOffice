import type { Achievement, AchievementApprove, AchievementApprovalSummary } from "../types/achievement";

const API_URL = import.meta.env.VITE_API_URL + "/achievements";

export interface CreateAchievementDto {
    title: string;
    description?: string;
}

export interface UpdateAchievementDto {
    title: string;
    description?: string;
}

export interface AchievementApprovalsDetails {
    approved: AchievementApproveDetails[];
    denied: AchievementApproveDetails[];
}

export interface AchievementApproveDetails {
    achievementApproveId: string;
    achievementId: string;
    userId: string;
    userLogin: string;
    userFirstName: string;
    userLastName: string;
    isApproved: boolean;
    approvedAt: string;
}

export const getAchievements = async (): Promise<Achievement[]> => {
    const response = await fetch(API_URL);

    if (!response.ok) {
        throw new Error("Failed to fetch achievements");
    }

    return response.json();
};

export const createAchievement = async (
    dto: CreateAchievementDto
): Promise<Achievement> => {
    const response = await fetch(API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(dto),
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);

        const message = error?.message ?? "Failed to create achievement";

        throw new Error(message);
    }

    return response.json();
};

export async function updateAchievement(
    id: string,
    dto: UpdateAchievementDto
) {
    const response = await fetch(
        `${API_URL}/${id}`,
        {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            credentials: "include",
            body: JSON.stringify(dto),
        }
    );

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        const message = error?.message ?? "Failed to update achievement";
        throw new Error(message);
    }
}

export const deleteAchievement = async (
    id: string
): Promise<void> => {
    const response = await fetch(`${API_URL}/${id}`, {
        method: "DELETE",
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to delete achievement");
    }
};

export const approveAchievement = async (
    achievementId: string,
    isApproved: boolean
): Promise<AchievementApprove> => {
    const response = await fetch(`${API_URL}/${achievementId}/approve`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify({ achievementId, isApproved }),
    });
    if (!response.ok) {
        const error = await response.json().catch(() => null);
        const message = error?.message ?? "Failed to approve achievement";
        throw new Error(message);
    }
    return response.json();
};

export const getApprovalSummary = async (
    achievementId: string
): Promise<AchievementApprovalSummary> => {
    const response = await fetch(`${API_URL}/${achievementId}/approvals/summary`, {
        credentials: "include"
    });
    if (!response.ok) {
        throw new Error("Failed to fetch approval summary");
    }
    return response.json();
};

export const getAchievementApprovalsDetails = async (
    achievementId: string
): Promise<AchievementApprovalsDetails> => {
    const response = await fetch(
        `${API_URL}/${achievementId}/approvals/grouped`,
        {
            credentials: "include"
        }
    );

    if (!response.ok) {
        throw new Error("Failed to fetch approvals details");
    }

    return response.json();
};