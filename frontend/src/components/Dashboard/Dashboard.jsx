import React, { useEffect, useState } from "react";
import { getPeople, getAlerts, deleteAlert } from "../../Services/api";
import "bootstrap/dist/css/bootstrap.min.css";

/**
 * Dashboard Component
 * Main intelligence dashboard displaying key metrics, quality informants,
 * high-risk targets, and active alerts.
 */
export default function Dashboard() {
    // State management for people data, alerts, loading state, and error handling
    const [people, setPeople] = useState([]);
    const [alerts, setAlerts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Fetch data on component mount
    useEffect(() => {
        const fetchData = async () => {
            try {
                // Fetch both people and alerts data concurrently
                const [peopleData, alertsData] = await Promise.all([
                    getPeople(),
                    getAlerts(),
                ]);
                setPeople(peopleData);
                setAlerts(alertsData);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    // Filter and categorize data for display
    const stars = people.filter((p) => p.RecruitScore > 0.7); // Quality informants with high recruit scores
    const dangerous = people.filter(
        (p) => p.RiskLevel === "HIGH" || p.RiskLevel === "CRITICAL"
    ); // High-risk targets
    const activeAlerts = alerts.filter((a) => a.Status !== "RESOLVED"); // Non-resolved alerts

    const handleDeleteAlert = async (id) => {
        if (!window.confirm("Are you sure you want to delete this alert?")) return;
        try {
            await deleteAlert(id);
            setAlerts((prev) => prev.filter((a) => a.Id !== id));
        } catch (err) {
            setError(err.message || "Failed to delete alert");
        }
    };

    return (
        <div className="container mt-4">
            <h2 className="mb-4">Malshinon Intelligence Dashboard</h2>

            {/* Key Metrics Section - Displaying summary cards */}
            <div className="row mb-4">
                {/* Total People Card */}
                <div className="col-md-3">
                    <div className="card text-bg-primary">
                        <div className="card-body text-center">
                            <h3 className="card-title">{people.length}</h3>
                            <p className="card-text">Total People</p>
                        </div>
                    </div>
                </div>
                {/* Quality Informants Card */}
                <div className="col-md-3">
                    <div className="card text-bg-success">
                        <div className="card-body text-center">
                            <h3 className="card-title">{stars.length}</h3>
                            <p className="card-text">Quality Informants</p>
                        </div>
                    </div>
                </div>
                {/* High Risk Targets Card */}
                <div className="col-md-3">
                    <div className="card text-bg-danger">
                        <div className="card-body text-center">
                            <h3 className="card-title">{dangerous.length}</h3>
                            <p className="card-text">High Risk Targets</p>
                        </div>
                    </div>
                </div>
                {/* Active Alerts Card */}
                <div className="col-md-3">
                    <div className="card text-bg-warning">
                        <div className="card-body text-center">
                            <h3 className="card-title">{activeAlerts.length}</h3>
                            <p className="card-text">Active Alerts</p>
                        </div>
                    </div>
                </div>
            </div>

            {/* Quality Informants Section - Displaying high-scoring informants */}
            <div className="card mb-4">
                <div className="card-header bg-success text-white">
                    <h4 className="mb-0">Quality Informants</h4>
                </div>
                <div className="card-body">
                    {loading ? (
                        <div className="alert alert-info">Loading...</div>
                    ) : error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : (
                        <div className="list-group">
                            {stars.map((person) => (
                                <div key={person.Id} className="list-group-item">
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h5 className="mb-1">{person.Name}</h5>
                                            <small className="text-muted">
                                                Code: {person.SecretCode}
                                            </small>
                                        </div>
                                        <div>
                                            <span className="badge bg-success">
                                                Score: {(person.RecruitScore ?? 0).toFixed(2)}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>

            {/* High Risk Targets Section - Displaying dangerous individuals */}
            <div className="card mb-4">
                <div className="card-header bg-danger text-white">
                    <h4 className="mb-0">High Risk Targets</h4>
                </div>
                <div className="card-body">
                    {loading ? (
                        <div className="alert alert-info">Loading...</div>
                    ) : error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : (
                        <div className="list-group">
                            {dangerous.map((person) => (
                                <div key={person.Id} className="list-group-item">
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h5 className="mb-1">{person.Name}</h5>
                                            <small className="text-muted">
                                                Code: {person.SecretCode}
                                            </small>
                                        </div>
                                        <div>
                                            <span
                                                className={`badge bg-${person.RiskLevel === "CRITICAL" ? "danger" : "warning"}`}
                                            >
                                                {person.RiskLevel}
                                            </span>
                                            <span className="ms-2 badge bg-secondary">
                                                Threat: {(person.ThreatScore ?? 0).toFixed(2)}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>

            {/* Active Alerts Section - Displaying current alerts */}
            <div className="card">
                <div className="card-header bg-warning">
                    <h4 className="mb-0">Active Alerts</h4>
                </div>
                <div className="card-body">
                    {loading ? (
                        <div className="alert alert-info">Loading...</div>
                    ) : error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : (
                        <div className="list-group">
                            {activeAlerts.map((alert) => (
                                <div key={alert.Id} className="list-group-item">
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h5 className="mb-1">{alert.Title}</h5>
                                            <p className="mb-1">{alert.Description}</p>
                                            <small className="text-muted">
                                                Created: {new Date(alert.CreatedAt).toLocaleString()}
                                            </small>
                                        </div>
                                        <span>
                                            <span className={`badge bg-${(alert.Severity || '').toLowerCase()}`}>{alert.Severity}</span>
                                            <button
                                                className="btn btn-outline-danger btn-sm ms-3"
                                                onClick={() => handleDeleteAlert(alert.Id)}
                                            >
                                                Delete
                                            </button>
                                        </span>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}
