import { useState, useEffect } from "react";
import {
    createAchievement,
    updateAchievement,
    type CreateAchievementDto,
} from "../../api/achievementApi";
import type { Achievement } from "../../types/achievement";

interface Props {
    onAchievementCreated: () => void;
    achievement?: Achievement;
}

export default function AchievementForm({
    onAchievementCreated,
    achievement,
}: Props) {
    const [title, setTitle] = useState(
        achievement?.title ?? ""
    );
    const [description, setDescription] = useState(
        achievement?.description ?? ""
    );
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
        const [visibilityId, setVisibilityId] = useState<number>(
        achievement?.visibilityId ?? 1
    );
    const [groupIds, setGroupIds] = useState<string[]>(achievement?.groupIds ?? []);
    const [groups, setGroups] = useState<{ groupId: string; name: string }[]>([]);
    
    useEffect(() => {
        const fetchGroups = async () => {
            try {
                const res = await fetch(import.meta.env.VITE_API_URL + "/groups", {
                    credentials: "include",
                });

                if (!res.ok) throw new Error("Failed to fetch groups");

                const data = await res.json();
                setGroups(data);
            } catch (e) {
                console.error(e);
            }
        };

        fetchGroups();
    }, []);

    const handleSubmit = async (
        e: React.FormEvent<HTMLFormElement>
    ) => {
        e.preventDefault();

        try {
            setLoading(true);
            setError(null);

            if (achievement) {
                await updateAchievement(
                    achievement.achievementId, 
                    {
                        title,
                        description,
                        visibilityId,
                        groupIds: visibilityId === 3 ? groupIds : [],
                    }
                );
            } else {
                const dto: CreateAchievementDto = {
                    title,
                    description,
                    visibilityId,
                    groupIds: visibilityId === 3 ? groupIds : [],
                };

                await createAchievement(dto);
            }

            onAchievementCreated();

            setTitle("");
            setDescription("");
        } catch (error: any) {
            console.error(error);
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>{achievement ? "Edit achievement" : "Add achievement"}</h2>

            <div>
                <input 
                    type="text"
                    placeholder="Title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                />
            </div>

            <div>
                <textarea
                    placeholder="Description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                />
            </div>

            <div className="form-group">
                <label htmlFor="visibility">Visibility</label>
                <select
                    id="visibility"
                    value={visibilityId}
                    onChange={(e) => setVisibilityId(Number(e.target.value))}
                >
                    <option value={1}>Public</option>
                    <option value={2}>Private</option>
                    <option value={3}>Groups</option>
                </select>
            </div>

            {visibilityId === 3 && (
                <div className="form-group">
                    <label>Allowed groups</label>

                    {groups.map((g) => (
                        <label key={g.groupId} style={{ display: "block" }}>
                            <input
                                type="checkbox"
                                checked={groupIds.includes(g.groupId)}
                                onChange={(e) => {
                                    if (e.target.checked) {
                                        setGroupIds([...groupIds, g.groupId]);
                                    } else {
                                        setGroupIds(groupIds.filter(id => id !== g.groupId));
                                    }
                                }}
                            />
                            {g.name}
                        </label>
                    ))}
                </div>
            )}

            {error && (
                <p style={{ color: "red", marginTop: "8px" }}>
                    {error}
                </p>
            )}

            <button type="submit" disabled={loading}>
                {loading ? achievement ? "Saving..." : "Adding..." : achievement ? "Save changes" : "Add achievement"}
            </button>
        </form>
    );
}