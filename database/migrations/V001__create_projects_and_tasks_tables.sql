CREATE TABLE IF NOT EXISTS projects (
    id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT NOT NULL,
    owner_user_id VARCHAR(450) NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS project_members (
    project_id UUID NOT NULL,
    user_id VARCHAR(450) NOT NULL,
    created_at TIMESTAMP NOT NULL,
    CONSTRAINT pk_project_members PRIMARY KEY (project_id, user_id),
    CONSTRAINT fk_project_members_projects_project_id
        FOREIGN KEY (project_id) REFERENCES projects (id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS project_tasks (
    id UUID PRIMARY KEY,
    project_id UUID NOT NULL,
    title VARCHAR(150) NOT NULL,
    description TEXT NOT NULL,
    status VARCHAR(20) NOT NULL,
    due_date TIMESTAMP NULL,
    created_at TIMESTAMP NOT NULL,
    completed_at TIMESTAMP NULL,
    CONSTRAINT fk_project_tasks_projects_project_id
        FOREIGN KEY (project_id) REFERENCES projects (id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_projects_owner_user_id ON projects(owner_user_id);
CREATE INDEX IF NOT EXISTS idx_project_members_user_id ON project_members(user_id);
CREATE INDEX IF NOT EXISTS idx_project_tasks_project_id ON project_tasks(project_id);
CREATE INDEX IF NOT EXISTS idx_project_tasks_project_id_status ON project_tasks(project_id, status);
