const API_URL = import.meta.env.VITE_API_URL + "/groups";

export interface Group {
    groupId: string;
    name: string;
    description: string;
    maxUserCount: number;
    avatarUrl?: string;
    createdAt: string;
}

export interface GroupRole {
    groupUserRoleId: string;
    name: string;
    isAdmin: boolean;
}

export interface GroupMember {
    userId: string;
    login: string;
    firstName: string;
    lastName: string;
    groupUserRoleId: string;
    roleName: string;
    isAdmin: boolean;
}

export interface CreateGroupRequest {
    name: string;
    description: string;
    maxUserCount: number;
}

export async function getGroups(): Promise<Group[]> {
    const res = await fetch(API_URL, { credentials: "include" });
    if (!res.ok) throw new Error("Failed to fetch groups");
    return res.json();
}

export async function getGroupById(groupId: string): Promise<Group> {
    const res = await fetch(`${API_URL}/${groupId}`, { credentials: "include" });
    if (!res.ok) throw new Error("Failed to fetch group details");
    return res.json();
}

export async function createGroup(data: CreateGroupRequest): Promise<{ groupId: string }> {
    const res = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
        credentials: "include"
    });
    if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(err.error || "Failed to create group");
    }
    return res.json();
}

export async function getGroupMembers(groupId: string): Promise<GroupMember[]> {
    const res = await fetch(`${API_URL}/${groupId}/members`, { credentials: "include" });
    if (!res.ok) throw new Error("Failed to fetch group members");
    return res.json();
}

export async function getGroupRoles(groupId: string): Promise<GroupRole[]> {
    const res = await fetch(`${API_URL}/${groupId}/roles`, { credentials: "include" });
    if (!res.ok) throw new Error("Failed to fetch group roles");
    return res.json();
}

export async function addGroupMember(groupId: string, userId: string, roleId: string): Promise<void> {
    const res = await fetch(`${API_URL}/${groupId}/members`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userId, roleId }),
        credentials: "include"
    });
    if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(err.error || "Failed to add member");
    }
}

export async function removeGroupMember(groupId: string, userId: string): Promise<void> {
    const res = await fetch(`${API_URL}/${groupId}/members/${userId}`, {
        method: "DELETE",
        credentials: "include"
    });
    if (!res.ok) throw new Error("Failed to remove member");
}