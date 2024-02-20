CREATE TABLE public.experiment
(
    id uuid NOT NULL,
    name character varying(200) COLLATE pg_catalog."default" NOT NULL,
    created timestamp with time zone NOT NULL DEFAULT now(),
    CONSTRAINT experiment_pkey PRIMARY KEY (id)
);

CREATE TABLE public.carddefinition
(
    name character varying(200) COLLATE pg_catalog."default" NOT NULL,
    power integer NOT NULL,
    cost integer NOT NULL
);