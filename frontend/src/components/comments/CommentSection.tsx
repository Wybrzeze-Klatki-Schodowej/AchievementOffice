import { useEffect, useState } from "react";
import { createComment, getComments, updateComment, deleteComment } from "../../api/CommentApi";
import { adminDeleteComment } from "../../api/AdminApi";
import type { Comment } from "../../types/comment";
import { getCurrentUser } from "../../api/LoginApi";
import "./CommentSection.css";

interface Props {
    profileUserId: string;
}

export default function CommentSection({
    profileUserId
}: Props) {
    const [comments, setComments] = useState<Comment[]>([]);
    const [content, setContent] = useState("");
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);
    const [currentUserRole, setCurrentUserRole] = useState<string | null>(null);
    const [editingCommentId, setEditingCommentId] = useState<string | null>(null);
    const [editContent, setEditContent] = useState("");

    async function loadComments() {
        try {
            const data = await getComments(profileUserId);
            setComments(data);
        } catch (err) {
            console.error(err);
        }
    }

    async function handleAddComment() {
        if (!content.trim()) {
            return;
        }

        try {
            await createComment(
                profileUserId,
                {
                    content
                }
            );
            setContent("");
            await loadComments();
        } catch (err) {
            console.error(err);
        }
    }

    async function handleSaveEdit(commentId: string) {
        try {
            await updateComment(
                commentId,
                {
                    content: editContent
                }
            );

            setEditingCommentId(null);
            setEditContent("");
            await loadComments();
        } catch (err) {
            console.error(err);
        }
    }

    async function handleDeleteComment(commentId: string) {
        try {
            if (currentUserRole === "Admin") {
                await adminDeleteComment(commentId);
            } else {
                await deleteComment(commentId);
            }
            await loadComments();
        } catch (err) {
            console.error(err);
        }
    }

    useEffect(() => {
        getCurrentUser()
            .then(user => {
                setCurrentUserId(user.userId);
                setCurrentUserRole(user.role);
            })
            .catch(console.error);
    }, []);

    useEffect(() => {
        loadComments();
    }, [profileUserId]);

    return (
        <div className="profile-card comments-section">
            <h2>Comments</h2>

            <div className="comment-form">
                <textarea
                    value={content}
                    onChange={(e) => setContent(e.target.value)}
                    rows={4}
                    placeholder="Write a comment..."
                />
                <button onClick={handleAddComment}>Add Comment</button>
            </div>

            {comments.length === 0 && (
                <div className="no-comments">No comments yet.</div>
            )}

            {comments.map(comment => {
                const isAuthor = comment.authorId === currentUserId;
                const isAdmin = currentUserRole === "Admin";
                const isProfileOwner = comment.authorId === profileUserId;
                const wasEdited = comment.createdAt !== comment.updatedAt;

                return (
                    <div key={comment.id} className="comment-card">
                        <div className="comment-header">
                            <div className="comment-author">
                                <div className="comment-author-name">
                                    {comment.authorFirstName} {comment.authorLastName}
                                    {isProfileOwner && (
                                        <span className="comment-owner-badge">Profile owner</span>
                                    )}
                                </div>
                                <div className="comment-login">@{comment.authorLogin}</div>
                            </div>

                            <div className="comment-date">
                                {new Date(comment.createdAt).toLocaleString()}
                                {wasEdited && (
                                    <>
                                        <br />
                                        <small>Edited</small>
                                    </>
                                )}
                            </div>
                        </div>

                        {editingCommentId === comment.id ? (
                            <div className="comment-edit">
                                <textarea
                                    value={editContent}
                                    onChange={(e) => setEditContent(e.target.value)}
                                />
                                <div className="comment-edit-buttons">
                                    <button onClick={() => handleSaveEdit(comment.id)}>Save</button>
                                    <button onClick={() => {
                                        setEditingCommentId(null);
                                        setEditContent("");
                                    }}>Cancel</button>
                                </div>
                            </div>
                        ) : (
                            <div className="comment-content">{comment.content}</div>
                        )}

                        {editingCommentId !== comment.id && (isAuthor || isAdmin) && (
                            <div className="comment-actions">
                                {isAuthor && (
                                    <button onClick={() => {
                                        setEditingCommentId(comment.id);
                                        setEditContent(comment.content);
                                    }}>
                                        Edit
                                    </button>
                                )}
                                <button onClick={() => handleDeleteComment(comment.id)}>
                                    Delete
                                </button>
                            </div>
                        )}
                    </div>
                );
            })}
        </div>
    );
}