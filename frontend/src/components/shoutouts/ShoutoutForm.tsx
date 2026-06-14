import { useEffect, useState } from "react";
import {
    createShoutout,
    updateShoutout,
    type CreateShoutoutDto,
    type UpdateShoutoutDto,
} //from "../../api/ShoutoutsApi";
from "../../api/ShoutoutApi";
import type { Shoutout } from "../../types/shoutout";
import { getCurrentUser } from "../../api/LoginApi";

interface User {
    userId: string;
    login: string;
}

interface Props {
    onShoutoutCreated: () => void;
    shoutout?: Shoutout;
}

export default function ShoutoutForm({
    onShoutoutCreated,
    shoutout,
}: Props) {
    const [title, setTitle] = useState(shoutout?.title ?? "");
    const [description, setDescription] = useState(shoutout?.description ?? "");
    const [receiverId, setReceiverId] = useState("");
    const [users, setUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [visibilityId, setVisibilityId] = useState<number>(
        shoutout?.visibilityId ?? 1
    );
    const [groupIds, setGroupIds] = useState<string[]>(shoutout?.groupIds ?? []);
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

    useEffect(() => {
        if (shoutout) return;

        const fetchUsers = async () => {
            try {
                const response = await fetch(
                    import.meta.env.VITE_API_URL + "/users",
                    {
                        credentials: "include",
                    }
                );

                console.log("GET users status:", response.status);

                if (!response.ok) {
                    throw new Error("Failed to fetch users");
                }

                const data: User[] = await response.json();

                const current = await getCurrentUser();

                const filteredUsers = data.filter(u => u.userId !== current.userId);
                setUsers(filteredUsers);

                if (filteredUsers.length > 0) {
                    setReceiverId(filteredUsers[0].userId);
                }
            } catch (error) {
                console.error("FETCH USERS ERROR:", error);
            }
        };

        fetchUsers();
    }, [shoutout]);

    const handleSubmit = async (
        e: React.FormEvent<HTMLFormElement>
    ) => {
        e.preventDefault();

        try {
            setLoading(true);

            if (!shoutout && !receiverId) {
                throw new Error("No receiver selected");
            }

            if (shoutout) {
                const updateDto: UpdateShoutoutDto = {
                    title,
                    description,
                    visibilityId,
                    groupIds: visibilityId === 3 ? groupIds : [],
                };
                await updateShoutout(shoutout.shoutoutId, updateDto);
            } else {
                const dto: CreateShoutoutDto = {
                    receiverId,
                    title,
                    description,
                    visibilityId,
                    groupIds: visibilityId === 3 ? groupIds : undefined,
                };

                console.log("Creating shoutout:", dto);

                await createShoutout(dto);
            }

            onShoutoutCreated();

            setTitle("");
            setDescription("");
            setError(null);

            if (users.length > 0) {
                setReceiverId(users[0].userId);
            }
        } catch (error) {
            console.error("SHOUTOUT ERROR:", error);
            setError(error instanceof Error ? error.message : "Operation failed");
        } finally {
            setLoading(false);
        }
    };

        return (
        <form onSubmit={handleSubmit} className="shoutout-form">
            <h2>{shoutout ? "Edit shoutout" : "Add shoutout"}</h2>

            {!shoutout && (
                <div className="form-group">
                    <label htmlFor="receiver">Receiver</label>
                    <select
                        id="receiver"
                        value={receiverId}
                        onChange={(e) => setReceiverId(e.target.value)}
                        required
                    >
                        <option value="">Select user</option>

                        {users.map((user) => (
                            <option key={user.userId} value={user.userId}>
                                {user.login}
                            </option>
                        ))}
                    </select>
                </div>
            )}

            <div className="form-group">
                <label htmlFor="title">{shoutout ? "Edit title" : "Title"}</label>
                <input
                    id="title"
                    type="text"
                    placeholder="Title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    required
                    maxLength={100}
                    className={error ? "error" : ""}
                />
            </div>

            <div className="form-group">
                <label htmlFor="description">{shoutout ? "Edit description" : "Description"}</label>
                <textarea
                    id="description"
                    placeholder="Description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    required
                    maxLength={500}
                    className={error ? "error" : ""}
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

            {error && <div className="error-message">{error}</div>}

            <button type="submit" disabled={loading} className="submit-btn">
                {loading
                    ? shoutout
                        ? "Saving..."
                        : "Adding..."
                    : shoutout
                        ? "Save changes"
                        : "Add shoutout"}
            </button>
        </form>
    );
}