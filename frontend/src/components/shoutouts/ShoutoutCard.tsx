import type { Shoutout } from "../../types/shoutout";
import { useState, useEffect } from "react";

interface Props {
    shoutout: Shoutout;
    currentUserId: string | null;
}

export default function ShoutoutCard({ shoutout, currentUserId }: Props) {

    return (
        <div
            style={{
                border: "1px solid #ccc",
                padding: "16px",
                borderRadius: "8px",
                marginBottom: "12px",
            }}
        >
            <h3>{shoutout.title}</h3>

            <p>{shoutout.description}</p>

            <small>
                Created: {new Date(shoutout.createdAt).toLocaleString()}
                <br />
                Updated: {new Date(shoutout.updatedAt).toLocaleString()}
            </small>

        </div>
    );
}