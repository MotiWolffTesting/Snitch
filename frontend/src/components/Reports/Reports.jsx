import React, { useEffect, useState } from "react";
import { getReports, updateReport, deleteReport } from "../../Services/api";

export default function Reports() {
    const [reports, setReports] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [editingId, setEditingId] = useState(null);
    const [editText, setEditText] = useState("");

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
        setEditingId(report.Id);
        setEditText(report.ReportText);
    };

    const handleEditSave = async (id) => {
        try {
            await updateReport(id, {
                ...reports.find((r) => r.Id === id),
                ReportText: editText,
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
                    <li key={r.Id} className="list-group-item">
                        <div className="fw-bold">
                            Reporter: {r.ReporterId} → Target: {r.TargetId}
                        </div>
                        {editingId === r.Id ? (
                            <>
                                <textarea
                                    className="form-control mb-2"
                                    value={editText}
                                    onChange={(e) => setEditText(e.target.value)}
                                />
                                <button
                                    className="btn btn-success btn-sm me-2"
                                    onClick={() => handleEditSave(r.Id)}
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
                                    <div>{r.ReportText}</div>
                                <div className="text-muted small">
                                        Submitted: {new Date(r.SubmittedAt).toLocaleString()}
                                </div>
                                <button
                                    className="btn btn-outline-primary btn-sm me-2 mt-2"
                                    onClick={() => handleEdit(r)}
                                >
                                    Edit
                                </button>
                                <button
                                    className="btn btn-outline-danger btn-sm mt-2"
                                        onClick={() => handleDelete(r.Id)}
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
