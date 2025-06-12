import React, { useState } from "react";
import { getPeople, getPerson } from "../../Services/api";

const roleLabels = {
    0: "Agent",
    1: "Terrorist",
    2: "Intel",
    3: "Civilian",
    Agent: "Agent",
    Terrorist: "Terrorist",
    Intel: "Intel",
    Civilian: "Civilian",
};

export default function PersonLookup() {
    const [search, setSearch] = useState("");
    const [roleFilter, setRoleFilter] = useState("");
    const [results, setResults] = useState([]);
    const [selected, setSelected] = useState(null);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);

    const handleSearch = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        setSelected(null);
        try {
            const people = await getPeople();
            setResults(
                people.filter((p) => {
                    const name = p.Name || p.name || "";
                    const matchesName = name.toLowerCase().includes(search.toLowerCase());
                    const role = p.Role || p.role;
                    const matchesRole =
                        roleFilter === "" || String(role) === roleFilter;
                    return matchesName && matchesRole;
                })
            );
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleSelect = async (id) => {
        setLoading(true);
        setError(null);
        try {
            const person = await getPerson(id);
            setSelected(person);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mt-4">
            <h2 className="mb-4">Lookup Individuals</h2>
            <form className="row g-3 mb-3" onSubmit={handleSearch}>
                <div className="col-md-5">
                    <input
                        className="form-control"
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        placeholder="Enter name..."
                    />
                </div>
                <div className="col-md-4">
                    <select
                        className="form-select"
                        value={roleFilter}
                        onChange={(e) => setRoleFilter(e.target.value)}
                    >
                        <option value="">All Roles</option>
                        <option value="0">Agent</option>
                        <option value="1">Terrorist</option>
                        <option value="2">Intel</option>
                        <option value="3">Civilian</option>
                    </select>
                </div>
                <div className="col-md-3">
                    <button
                        type="submit"
                        className="btn btn-primary w-100"
                        disabled={loading}
                    >
                        Search
                    </button>
                </div>
            </form>
            {error && <div className="alert alert-danger">{error}</div>}
            <ul className="list-group mb-3">
                {results.map((p) => (
                    <li
                        key={p.Id || p.id}
                        className="list-group-item d-flex justify-content-between align-items-center"
                    >
                        <span>
                            <span className="fw-bold">{p.Name || p.name}</span>{" "}
                            <span className="badge bg-secondary ms-2">
                                {roleLabels[p.Role || p.role] || "Unknown"}
                            </span>{" "}
                            <span className="text-muted ms-2">(ID: {p.Id || p.id})</span>
                        </span>
                        <button
                            className="btn btn-outline-secondary btn-sm"
                            onClick={() => handleSelect(p.Id || p.id)}
                        >
                            View
                        </button>
                    </li>
                ))}
            </ul>
            {selected && (
                <div className="card mt-4">
                    <div className="card-header">{roleLabels[selected.Role || selected.role] || "Unknown"} Details</div>
                    <div className="card-body">
                        {/* Common fields */}
                        <div>
                            <b>Name:</b> {selected.Name || selected.name || "-"}
                        </div>
                        <div>
                            <b>Role:</b> {roleLabels[selected.Role || selected.role] || "Unknown"}
                        </div>
                        <div>
                            <b>Secret Code:</b> {selected.SecretCode || selected.secretCode || "-"}
                        </div>
                        <div>
                            <b>Created At:</b> {selected.CreatedAt || selected.createdAt ? (new Date(selected.CreatedAt || selected.createdAt).toString() !== "Invalid Date" ? new Date(selected.CreatedAt || selected.createdAt).toLocaleString() : "-") : "-"}
                        </div>
                        <div>
                            <b>Updated At:</b> {selected.UpdatedAt || selected.updatedAt ? (new Date(selected.UpdatedAt || selected.updatedAt).toString() !== "Invalid Date" ? new Date(selected.UpdatedAt || selected.updatedAt).toLocaleString() : "-") : "-"}
                        </div>

                        {/* Role-specific information */}
                        {(selected.Role === 0 || selected.Role === "Agent" || selected.role === 0 || selected.role === "Agent") && (
                            <>
                                <div>
                                    <b>Reports Made:</b> {selected.TotalReportsMade || selected.totalReportsMade || "-"}
                                </div>
                                <div>
                                    <b>Reporting Consistency:</b> {selected.ReportingConsistency || selected.reportingConsistency || "-"}
                                </div>
                                <div>
                                    <b>Network Centrality:</b> {selected.NetworkCentrality || selected.networkCentrality || "-"}
                                </div>
                                <div>
                                    <b>Influence Score:</b> {selected.InfluenceScore || selected.influenceScore || "-"}
                                </div>
                            </>
                        )}
                        {(selected.Role === 1 || selected.Role === "Terrorist" || selected.role === 1 || selected.role === "Terrorist") && (
                            <>
                                <div>
                                    <b>Risk Level:</b> {selected.RiskLevel || selected.riskLevel || "-"}
                                </div>
                                <div>
                                    <b>Threat Score:</b> {selected.ThreatScore || selected.threatScore || "-"}
                                </div>
                                <div>
                                    <b>Network Centrality:</b> {selected.NetworkCentrality || selected.networkCentrality || "-"}
                                </div>
                            </>
                        )}
                        {(selected.Role === 2 || selected.Role === "Intel" || selected.role === 2 || selected.role === "Intel") && (
                            <>
                                <div>
                                    <b>Reports Made:</b> {selected.TotalReportsMade || selected.totalReportsMade || "-"}
                                </div>
                                <div>
                                    <b>Reports Received:</b> {selected.TotalReportsReceived || selected.totalReportsReceived || "-"}
                                </div>
                                <div>
                                    <b>Network Centrality:</b> {selected.NetworkCentrality || selected.networkCentrality || "-"}
                                </div>
                                <div>
                                    <b>Influence Score:</b> {selected.InfluenceScore || selected.influenceScore || "-"}
                                </div>
                            </>
                        )}
                        {(selected.Role === 3 || selected.Role === "Civilian" || selected.role === 3 || selected.role === "Civilian") && (
                            <>
                                <div>
                                    <b>Risk Level:</b> {selected.RiskLevel || selected.riskLevel || "-"}
                                </div>
                                <div>
                                    <b>Recruit Score:</b> {selected.RecruitScore || selected.recruitScore || "-"}
                                </div>
                                <div>
                                    <b>Reports Received:</b> {selected.TotalReportsReceived || selected.totalReportsReceived || "-"}
                                </div>
                            </>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}
