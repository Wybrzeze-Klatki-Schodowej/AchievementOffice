const API_URL = import.meta.env.VITE_API_URL + "/ranking";

export async function getUserRanking() {
    try{
        const res = await fetch(`${API_URL}/users`, {
            credentials: "include"
        });

        if (!res.ok) {
            throw new Error("Failed to fetch users ranking");
        }

        return await res.json();
    } catch (err) {
        console.error(err);
        return [];
    }
}

export async function getGroupRanking() {
    try{
        const res = await fetch(`${API_URL}/groups`, {
            credentials: "include"
        });

        if (!res.ok) {
            throw new Error("Failed to fetch groups ranking");
        }

        return await res.json();
    } catch (err) {
        console.error(err);
        return [];
    }
}