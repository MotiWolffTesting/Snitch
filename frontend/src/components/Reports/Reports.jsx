import React, { useEffect, useState } from "react";
import { getReports, updateReport, deleteReport } from "../../Services/api";

export default function Reports() {
    const [reports, setReports] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [editingId, setEditingId] = useState(null);
    const [editText, setEditText] = useState("");
    const [deletingId, setDeletingId] = useState(null);

    useEffect(() => {
        fetchReports();
    }, []);

    const fetchReports = () => {
        setLoading(true);
        getReports()
            .then(setReports)
            .catch(setError)
            .finally(() => setLoading(false));
    };

    const handleEdit = (report) => {
        setEditingId(report.id);
        setEditText(report.reportText);
    };

    const handleEditSave = async (id) => {
        try {
            await updateReport(id, {
                ...reports.find((r) => r.id === id),
                reportText: editText,
            });
            setEditingId(null);
            setEditText("");
            fetchReports();
        } catch (err) {
            setError(err);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Are you sure you want to delete this report?")) return;
        try {
            await deleteReport(id);
            fetchReports();
        } catch (err) {
            setError(err);
        }
    };

    return (
        <div className="container mt-4">
            <h2 className="mb-4">Recent Reports</h2>
            {loading && <div className="alert alert-info">Loading reports...</div>}
            {error && (
                <div className="alert alert-danger">
                    {error.message || error.toString()}
                </div>
            )}
            <ul className="list-group">
                {reports.map((r) => (
                    <li key={r.id} className="list-group-item">
                        <div className="fw-bold">
                            Reporter: {r.reporterId} → Target: {r.targetId}
                        </div>
                        {editingId === r.id ? (
                            <>
                                <textarea
                                    className="form-control mb-2"
                                    value={editText}
                                    onChange={(e) => setEditText(e.target.value)}
                                />
                                <button
                                    className="btn btn-success btn-sm me-2"
                                    onClick={() => handleEditSave(r.id)}
                                >
                                    Save
                                </button>
                                <button
                                    className="btn btn-secondary btn-sm"
                                    onClick={() => setEditingId(null)}
                                >
                                    Cancel
                                </button>
                            </>
                        ) : (
                            <>
                                <div>{r.reportText}</div>
                                <div className="text-muted small">
                                    Submitted: {new Date(r.submittedAt).toLocaleString()}
                                </div>
                                <button
                                    className="btn btn-outline-primary btn-sm me-2 mt-2"
                                    onClick={() => handleEdit(r)}
                                >
                                    Edit
                                </button>
                                <button
                                    className="btn btn-outline-danger btn-sm mt-2"
                                    onClick={() => handleDelete(r.id)}
                                >
                                    Delete
                                </button>
                            </>
                        )}
                    </li>
                ))}
            </ul>
        </div>
    );
}
