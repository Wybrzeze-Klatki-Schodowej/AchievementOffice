import type { Comment } from '../types/comment';

const API_URL = import.meta.env.VITE_API_URL;

export interface CreateCommentRequest {
    content: string;
}

export interface UpdateCommentRequest {
    content: string;
}

export async function getComments(
    userId: string
): Promise<Comment[]> {
    const res = await fetch(
        `${API_URL}/users/${userId}/comments`, 
        {
            credentials: "include"
        }
    );

    if (!res.ok) {
        throw new Error(`Error fetching comments: ${res.status}`);
    }

    return res.json();
}

export async function createComment(
    userId: string,
    request: CreateCommentRequest
): Promise<Comment> {
    const res = await fetch(
        `${API_URL}/users/${userId}/comments`,
        {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(request)
        }
    );

    if (!res.ok) {
        throw new Error(`Error creating comment: ${res.status}`);
    }

    return res.json();
}

export async function updateComment(
    commentId: string,
    request: UpdateCommentRequest
): Promise<Comment> {
    const res = await fetch(
        `${API_URL}/comments/${commentId}`,
        {
            method: "PUT",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(request)
        }
    );

    if (!res.ok) {
        throw new Error(`Error updating comment: ${res.status}`);
    }

    return res.json();
}

export async function deleteComment(
    commentId: string
): Promise<void> {
    const res = await fetch(
        `${API_URL}/comments/${commentId}`,
        {
            method: "DELETE",
            credentials: "include"
        }
    );

    if (!res.ok) {
        throw new Error(`Error deleting comment: ${res.status}`);
    }
}