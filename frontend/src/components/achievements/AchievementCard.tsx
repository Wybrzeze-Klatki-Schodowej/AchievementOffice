import type { Achievement } from "../../types/achievement";

interface Props {
    achievement: Achievement;
}

export default function AchievementCard({ achievement }: Props) {
    return (
        <div
            style={{
                border: "1px solid #ccc",
                padding: "16px",
                borderRadius: "8px",
                marginBottom: "12px",
            }}
        >
            <h3>{achievement.title}</h3>

            <p>{achievement.description}</p>

            <small>
                Created: {new Date(achievement.createdAt).toLocaleString()}
            </small>
        </div>
    );
}