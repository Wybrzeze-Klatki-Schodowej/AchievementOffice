import type { Shoutout, User } from "../types/shoutout";

const API_URL = import.meta.env.VITE_API_URL + "/shoutouts";

export interface CreateShoutoutDto {
    // senderId: string; // bierzemy z tokena po stronie backendu
    receiverId: string;
    title: string;
    description?: string;
}

export interface UpdateShoutoutDto {
    title: string;
    description?: string;
}

export const getShoutouts = async (): Promise<Shoutout[]> => {
    const response = await fetch(API_URL, {
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to fetch shoutouts");
    }

    return response.json();
};

export const getShoutoutById = async (id: string): Promise<Shoutout> => {
    const response = await fetch(`${API_URL}/${id}`, {
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to fetch shoutout");
    }

    return response.json();
};

export const createShoutout = async (dto: CreateShoutoutDto) => {
    const response = await fetch(API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(dto),
    });

    const text = await response.text();

    console.log("STATUS:", response.status);
    console.log("RESPONSE TEXT:", text);

    if (!response.ok) {
        throw new Error(
            `Failed to create shoutout: ${response.status} - ${text}`
        );
    }

    return text ? JSON.parse(text) : null;
};

export async function updateShoutout(
    id: string,
    dto: UpdateShoutoutDto
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
        throw new Error("Failed to update shoutout");
    }
}

export const deleteShoutout = async (
    id: string
): Promise<void> => {
    const response = await fetch(`${API_URL}/${id}`, {
        method: "DELETE",
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to delete shoutout");
    }
};

export const getUsers = async (): Promise<User[]> => {
    const response = await fetch(`${API_URL}/users`);

    if (!response.ok) {
        throw new Error("Failed to fetch users");
    }

    return response.json();
};

export const addReaction = async (shoutoutId: string, reactionType: number): Promise<Shoutout> => {
    const response = await fetch(`${API_URL}/${shoutoutId}/react`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(reactionType),
    });

    if (!response.ok) {
        throw new Error("Failed to add reaction");
    }

    return response.json();
};

export const removeReaction = async (shoutoutId: string): Promise<Shoutout> => {
    const response = await fetch(`${API_URL}/${shoutoutId}/react`, {
        method: "DELETE",
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to remove reaction");
    }

    return response.json();
};