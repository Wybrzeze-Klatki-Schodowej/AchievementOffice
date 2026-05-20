import type { UserProfile } from "../types/user";

const API_URL = "http://localhost:8080/api/users";

export interface UserListItem {
    userId: string;
    login: string;
    firstName: string;
    lastName: string;
}

export async function getUserProfile(
    userId: string
): Promise<UserProfile> {

    const res = await fetch(
        `${API_URL}/${userId}`, 
        {
            credentials: "include"
        }
    );

    if (!res.ok) {
        throw new Error(`Error fetching user profile: ${res.status}`);
    }

    return res.json();
}

export async function getUserAchievements(
    userId: string
) {
    const res = await fetch(
        `${API_URL}/${userId}/achievements`,
        {
            credentials: "include"
        }
    );

    if (!res.ok) {
        throw new Error(`Error fetching user achievements: ${res.status}`);
    }

    return res.json();
}

export async function getAllUsers(): Promise<UserListItem[]> {
    const res = await fetch(API_URL, {
        credentials: "include"
    });

    if (!res.ok) {
        throw new Error(`Error fetching users: ${res.status}`);
    }

    return res.json();
}