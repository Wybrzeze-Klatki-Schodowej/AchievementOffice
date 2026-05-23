import type { UserProfile } from "../types/user";

const API_URL = import.meta.env.VITE_API_URL + "/users";

export interface UserListItem {
    userId: string;
    login: string;
    firstName: string;
    lastName: string;
}

export interface UpdateUserRequest {
    email: string;
    username: string;
    firstname: string;
    lastname: string;
    jobTitle: string;
    bio?: string;
    avatarUrl?: string;
}

export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
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

export async function updateUserProfile(
    data: UpdateUserRequest
): Promise<UserProfile> {
    const res = await fetch(`${API_URL}/me`, {
        method: "PUT",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });

    if (!res.ok) {
        const error = await res.json().catch(() => null);
        throw new Error(error?.message ?? "Error updating profile");
    }

    return res.json();
}

export async function changePassword(
    data: ChangePasswordRequest
): Promise<void> {
    const res = await fetch(`${import.meta.env.VITE_API_URL}/users/me/password`, {
        method: "PUT",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });

    if (!res.ok) {
        const error = await res.json().catch(() => null);
        throw new Error(error?.message ?? "Error changing password");
    }
}