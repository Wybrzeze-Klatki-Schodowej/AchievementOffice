const ADMIN_API_URL = import.meta.env.VITE_API_URL + "/admin";

export interface AdminUserProfile {
    userId: string;
    login: string;
    firstName: string;
    lastName: string;
    email: string;
    jobTitle: string;
    bio?: string;
    avatarUrl?: string;
    role: string;
    rankId?: string;
    rankName?: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

export async function getAllUsers(isActive?: boolean): Promise<AdminUserProfile[]> {
    let url = `${ADMIN_API_URL}/users`;
    if (isActive !== undefined) {
        url += `?isActive=${isActive}`;
    }
    const response = await fetch(url, {
        method: "GET",
        credentials: "include"
    });

    if (!response.ok) {
        throw new Error(`Failed to fetch users: ${response.status}`);
    }

    return response.json();
}

export async function updateUserStatus(
    userId: string,
    isActive: boolean
): Promise<void> {
    const response = await fetch(
        `${ADMIN_API_URL}/users/${userId}/status`,
        {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                isActive
            }),
            credentials: "include"
        }
    );

    if (!response.ok) {
        const error = await response.json().catch(() => null);

        throw new Error(
            error?.message ??
            "Failed to update user status"
        );
    }
}

export interface RankResponse {
    id: string;
    name: string;
    multiplier: number;
}

export async function getRanks(): Promise<RankResponse[]> {
    const response = await fetch(`${ADMIN_API_URL}/ranks`, {
        credentials: "include"
    });
    if (!response.ok) {
        throw new Error(`Failed to fetch ranks: ${response.status}`);
    }
    return response.json();
}

export async function updateUserRank(
    userId: string,
    rankId: string | null
): Promise<void> {
    const response = await fetch(
        `${ADMIN_API_URL}/users/${userId}/rank`,
        {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                rankId
            }),
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error(`Failed to update user rank: ${response.status}`);
        }
}

export async function adminDeleteAchievement(
    achievementId: string
): Promise<void> {
    const response = await fetch(
        `${ADMIN_API_URL}/achievements/${achievementId}`, {
        method: "DELETE",
        credentials: "include"
    });
    if (!response.ok) {
        throw new Error(`Failed to delete achievement: ${response.status}`);
    }
}

export async function adminDeleteComment(commentId: string): Promise<void> {
    const response = await fetch(
        `${ADMIN_API_URL}/comments/${commentId}`,
        {
            method: "DELETE",
            credentials: "include"
        }
    );
    if (!response.ok) {
        throw new Error(`Failed to delete comment: ${response.status}`);
    }
}
