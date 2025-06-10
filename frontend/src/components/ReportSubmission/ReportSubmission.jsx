import React, { useState, useEffect } from "react";
import { createReport, getPeople, uploadReportsCsv } from "../../Services/api";
import { useNavigate } from "react-router-dom";

export default function ReportSubmission() {
    const [people, setPeople] = useState([]);
    const [reporterId, setReporterId] = useState("");
    const [targetId, setTargetId] = useState("");
    const [reportText, setReportText] = useState("");
    const [charCount, setCharCount] = useState(0);
    const [success, setSuccess] = useState(null);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);
    const [csvFile, setCsvFile] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        getPeople().then((data) => {
            setPeople(data);
            console.log("People loaded:", data);
            if (data.length > 0) {
                console.log("First person keys:", Object.keys(data[0]));
                console.log("First person full:", data[0]);
            }
        });
    }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccess("");
        if (reporterId === targetId) {
            setError("Reporter and Target must be different individuals.");
            return;
        }
        setLoading(true);
        try {
            await createReport({
                ReporterId: parseInt(reporterId, 10),
                TargetId: parseInt(targetId, 10),
                ReportText: reportText,
            });
            setSuccess("Report submitted successfully!");
            setReporterId("");
            setTargetId("");
            setReportText("");
            setTimeout(() => window.location.reload(), 1000);
        } catch (err) {
            setError(err.message || "Failed to submit report");
        } finally {
            setLoading(false);
        }
    };

    const handleFileUpload = async (e) => {
        const file = e.target.files[0];
        if (file && file.type === "text/csv") {
            setCsvFile(file);
            setError("");
        } else {
            setError("Please upload a valid CSV file");
            setCsvFile(null);
        }
    };

    const handleCsvUpload = async () => {
        if (!csvFile) return;

        setLoading(true);
        setError("");
        try {
            const reports = await uploadReportsCsv(csvFile);
            setSuccess(`Successfully imported ${reports.length} reports!`);
            setCsvFile(null);
            setTimeout(() => window.location.reload(), 1000);
        } catch (err) {
            setError(err.message || "Failed to upload CSV file");
        } finally {
            setLoading(false);
        }
    };

    const roleLabels = {
        0: "Agent",
        1: "Terrorist",
        2: "Intel",
        3: "Civilian",
    };

    // Filtered lists
    const agentPeople = people.filter((p) => p.role === "Agent");
    const targetPeople = people.filter(
        (p) => p.role === "Terrorist" || p.role === "Intel"
    );

    return (
        <div className="container mt-4">
            <h2 className="mb-4">Submit Intelligence Report</h2>

            {/* Privacy Notice */}
            <div className="alert alert-info mb-4">
                <h5 className="alert-heading">Privacy & Security</h5>
                <p className="mb-0">
                    <i className="bi bi-shield-lock me-2"></i>
                    All reports are encrypted and handled with strict confidentiality.
                    Your identity as a reporter is protected.
                </p>
            </div>

            <div className="row">
                {/* Manual Report Form */}
                <div className="col-md-8">
                    <div className="card shadow-lg border-0">
                        <div className="card-body p-5">
                            <h2 className="mb-4">Submit New Report</h2>
                            {error && <div className="alert alert-danger">{error}</div>}
                            {success && <div className="alert alert-success">{success}</div>}
                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label className="form-label">Reporter</label>
                                    <select
                                        className="form-select"
                                        value={reporterId}
                                        onChange={(e) => setReporterId(e.target.value)}
                                        required
                                    >
                                        <option value="">Select Reporter...</option>
                                        {agentPeople.map((p) => (
                                            <option key={p.id} value={p.id}>
                                                {p.name} ({roleLabels[p.role]}) [ID: {p.id}]
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="mb-3">
                                    <label className="form-label">Target</label>
                                    <select
                                        className="form-select"
                                        value={targetId}
                                        onChange={(e) => setTargetId(e.target.value)}
                                        required
                                    >
                                        <option value="">Select Target...</option>
                                        {targetPeople.map((p) => (
                                            <option key={p.id} value={p.id}>
                                                {p.name} ({roleLabels[p.role]}) [ID: {p.id}]
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="mb-3">
                                    <label className="form-label">Report Text</label>
                                    <textarea
                                        className="form-control"
                                        value={reportText}
                                        onChange={(e) => {
                                            setReportText(e.target.value);
                                            setCharCount(e.target.value.length);
                                        }}
                                        required
                                        maxLength={4000}
                                        rows={5}
                                    />
                                    <div className="text-end small text-muted">
                                        {charCount}/4000 characters
                                    </div>
                                </div>
                                <button
                                    type="submit"
                                    className="btn btn-primary w-100"
                                    disabled={loading}
                                >
                                    {loading ? "Submitting..." : "Submit Report"}
                                </button>
                            </form>
                        </div>
                    </div>
                </div>

                {/* CSV Upload */}
                <div className="col-md-4">
                    <div className="card p-4 shadow-sm">
                        <h5 className="card-title mb-3">Bulk Upload</h5>
                        <p className="text-muted small mb-2">
                            Upload multiple reports using a CSV file. The file should contain:
                        </p>
                        <ul className="text-muted small mb-3">
                            <li>Reporter ID</li>
                            <li>Target ID</li>
                            <li>Report Text</li>
                        </ul>
                        <div className="mb-3">
                            <input
                                type="file"
                                className="form-control"
                                accept=".csv"
                                onChange={handleFileUpload}
                            />
                        </div>
                        <button
                            className="btn btn-outline-primary w-100"
                            disabled={!csvFile || loading}
                            onClick={handleCsvUpload}
                        >
                            {loading ? "Uploading..." : "Upload CSV"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
