import type { UserProfile } from "../types/user";

const API_URL = "http://localhost:8080/api/users";

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