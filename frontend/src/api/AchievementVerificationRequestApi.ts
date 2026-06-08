import type {
    AchievementVerificationRequest,
    UserReviewer
} from "../types/achievementVerificationRequest";

const API_URL = import.meta.env.VITE_API_URL;

export interface CreateAchievementVerificationRequestDto {
    targetUserId: string;
}

export async function createVerificationRequest(
    achievementId: string,
    dto: CreateAchievementVerificationRequestDto
): Promise<AchievementVerificationRequest>
{
    const response = await fetch(
        `${API_URL}/achievements/${achievementId}/verification-requests`,
        {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(dto)
        });

    if (!response.ok)
    {
        const error = 
            await response.json().catch(() => null);

        throw new Error(
            error?.message ??
            "Failed to create verification request");
    }

    return response.json();
}

export async function approveVerificationRequest(
    requestId: string
): Promise<void>
{
    const response = await fetch(
        `${API_URL}/verification-requests/${requestId}/approve`,
        {
            method: "POST",
            credentials: "include"
        });

    if (!response.ok)
    {
        const error =
            await response.json().catch(() => null);

        throw new Error(
            error?.message ??
            "Failed to approve verification request");
    }
}

export async function rejectVerificationRequest(
    requestId: string
): Promise<void>
{
    const response = await fetch(
        `${API_URL}/verification-requests/${requestId}/reject`,
        {
            method: "POST",
            credentials: "include"
        });

    if (!response.ok)
    {
        const error = 
            await response.json().catch(() => null);

        throw new Error(
            error?.message ??
            "Failed to reject verification request");
    }
}

export async function getPendingVerificationRequests():
    Promise<AchievementVerificationRequest[]>
{
    const response = await fetch(
        `${API_URL}/verification-requests/pending`,
        {
            credentials: "include"
        });

    if (!response.ok)
    {
        throw new Error(
            "Failed to fetch pending verification requests");
    }

    return response.json();
}

export async function getAvailableReviewers(
    achievementId: string
): Promise<UserReviewer[]>
{
    const response = await fetch(
        `${API_URL}/achievements/${achievementId}/available-reviewers`,
        {
            credentials: "include"
        });

    if (!response.ok)
    {
        const error = 
            await response.json().catch(() => null);

        throw new Error(
            error?.message ??
            "Failed to fetch reviewers");
    }

    return response.json();
}