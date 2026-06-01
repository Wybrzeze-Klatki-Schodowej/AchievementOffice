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