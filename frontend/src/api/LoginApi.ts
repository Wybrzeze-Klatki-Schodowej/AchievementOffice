

const API_URL = "http://localhost:8080/api/Auth";

export interface LoginDTO {
    Login: string;
    Password: string;
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

    if (!res.ok) {
        const errMessage = await res.json().catch(() => ({}));
        throw new Error(errMessage.message || "Login failed");
    }
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