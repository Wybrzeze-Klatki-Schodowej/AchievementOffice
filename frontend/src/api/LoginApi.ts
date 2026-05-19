const API_URL = "http://localhost:8080/api/Auth";

export interface LoginDTO {
    Login: string;
    Password: string;
}

export interface RegisterDTO {
    Email: string;
    Username: string;
    Password: string;
    Firstname: string;
    Lastname: string;
    JobTitle: string;
    RoleName?: string;
}

async function handleResponse(res: Response, fallbackMessage: string) {
    if (!res.ok) {
        const errMessage = await res.json().catch(() => ({}));
        throw new Error(errMessage.message || fallbackMessage);
    }
}

export async function login(data: LoginDTO): Promise<void> {
    const res = await fetch(API_URL + "/login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data),
        credentials: "include"
    });

    await handleResponse(res, "Login failed");
}

export async function register(data: RegisterDTO): Promise<void> {
    const res = await fetch(API_URL + "/register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data),
        credentials: "include"
    });

    await handleResponse(res, "Registration failed");
}

export async function checkAuth(): Promise<boolean> {
    try {
        const res = await fetch(API_URL + "/is-logged", {
            method: "GET",
            credentials: "include"
        });

        if (!res.ok) return false;

        const data = await res.json();
        return data;
    } catch (err) {
        return false;
    }
}

export async function logout(): Promise<void> {
    try {
        await fetch(API_URL + "/logout", {
            method: "POST",
            credentials: "include"
        });

        window.location.href = "/login";
    } catch (err) {
        console.error("Error occurred while logging out:", err);
    }
}

export const getCurrentUser = async () => {
    const res = await fetch(API_URL + "/me", {
        credentials: "include"
    });

    if (!res.ok) {
        throw new Error("Failed to fetch current user");
    }

    return res.json();
}