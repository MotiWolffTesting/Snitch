import React, { useEffect, useState } from "react";
import axios from "axios";

const API_URL = process.env.REACT_APP_API_URL || "http://localhost:5243/api";

export default function AdminDashboard() {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [actionMsg, setActionMsg] = useState("");

    const fetchUsers = async () => {
        setLoading(true);
        setError("");
        try {
            const res = await axios.get(`${API_URL}/auth/users`);
            setUsers(res.data);
        } catch (err) {
            setError("Failed to load users");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchUsers();
    }, []);

    const handlePromote = async (id) => {
        setActionMsg("");
        try {
            await axios.post(`${API_URL}/auth/promote/${id}`);
            setActionMsg("User promoted to admin.");
            fetchUsers();
        } catch {
            setActionMsg("Failed to promote user.");
        }
    };

    const handleDemote = async (id) => {
        setActionMsg("");
        try {
            await axios.post(`${API_URL}/auth/demote/${id}`);
            setActionMsg("User demoted from admin.");
            fetchUsers();
        } catch {
            setActionMsg("Failed to demote user.");
        }
    };

    const handleDelete = async (id) => {
        setActionMsg("");
        try {
            await axios.delete(`${API_URL}/auth/${id}`);
            setActionMsg("User deleted.");
            fetchUsers();
        } catch {
            setActionMsg("Failed to delete user.");
        }
    };

    return (
        <div className="container mt-4">
            <h2>Admin Dashboard - User Management</h2>
            {error && <div className="alert alert-danger">{error}</div>}
            {actionMsg && <div className="alert alert-info">{actionMsg}</div>}
            {loading ? (
                <div>Loading users...</div>
            ) : (
                <table className="table table-bordered mt-3">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Username</th>
                            <th>Admin</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map((user) => (
                            <tr key={user.id}>
                                <td>{user.id}</td>
                                <td>{user.username}</td>
                                <td>{user.isAdmin ? "Yes" : "No"}</td>
                                <td>
                                    {user.isAdmin ? (
                                        <button
                                            className="btn btn-warning btn-sm me-2"
                                            onClick={() => handleDemote(user.id)}
                                        >
                                            Demote
                                        </button>
                                    ) : (
                                        <button
                                            className="btn btn-success btn-sm me-2"
                                            onClick={() => handlePromote(user.id)}
                                        >
                                            Promote
                                        </button>
                                    )}
                                    <button
                                        className="btn btn-danger btn-sm"
                                        onClick={() => handleDelete(user.id)}
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}
