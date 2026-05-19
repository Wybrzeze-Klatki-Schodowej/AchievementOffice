const API_URL = "http://localhost:8080/api/Auth";

export interface RegisterDTO {
    Email: string;
    Username: string;
    Password: string;
    Firstname: string;
    Lastname: string;
    JobTitle: string;
    RoleName?: string;
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
        
        if (errData?.errors) {
            const firstErrorKey = Object.keys(errData.errors)[0];
            throw new Error(errData.errors[firstErrorKey][0]);
        } else if (errData?.title) {
            throw new Error(errData.title);
        } else {
            throw new Error(errData?.message || "Registration failed");
        }
    }
}