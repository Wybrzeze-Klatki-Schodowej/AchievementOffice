import type { Achievement } from "../types/achievement";

const API_URL = "http://localhost:8080/api/achievements";

export interface CreateAchievementDto {
    userId: string;
    title: string;
    description?: string;
}

export interface UpdateAchievementDto {
    title: string;
    description?: string;
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
        body: JSON.stringify(dto),
    });

    if (!response.ok) {
        throw new Error("Failed to create achievement");
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
            body: JSON.stringify(dto),
        }
    );

    if (!response.ok) {
        throw new Error("Failed to update achievement");
    }
}

export const deleteAchievement = async (
    id: string
): Promise<void> => {
    const response = await fetch(`${API_URL}/${id}`, {
        method: "DELETE",
    });

    if (!response.ok) {
        throw new Error("Failed to delete achievement");
    }
};