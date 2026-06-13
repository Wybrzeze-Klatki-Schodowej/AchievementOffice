const API_URL = import.meta.env.VITE_API_URL + "/Auth";

export interface RegisterDTO {
    Email: string;
    Username: string;
    Password: string;
    Firstname: string;
    Lastname: string;
    JobTitle: string;
    RoleName?: string;
}

function getErrorMessage(error: any, defaultMessage: string): string {
    if (error?.message) {
        return error.message;
    }

    if (error?.errors) {
        return Object.values(error.errors)
            .flat()
            .join("\n");
    }

    return defaultMessage;
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

    if (!res.ok) {
        const errData = await res.json().catch(() => null);
        
        throw new Error(
            getErrorMessage(errData, "Error registering user")
        );
    }
}