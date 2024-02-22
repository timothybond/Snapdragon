CREATE TABLE public.experiment
(
    id uuid NOT NULL,
    name character varying(200) COLLATE pg_catalog."default" NOT NULL,
    created timestamp with time zone NOT NULL DEFAULT now(),
    CONSTRAINT experiment_pkey PRIMARY KEY (id)
);

CREATE TABLE public.population
(
    id uuid NOT NULL,
    experimentid uuid NOT NULL,
    name character varying(200) COLLATE pg_catalog."default" NOT NULL,
    controller character varying(200) COLLATE pg_catalog."default" NOT NULL,
    mutationper integer NOT NULL,
    orderby character varying(200) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT population_pkey PRIMARY KEY (id),
    CONSTRAINT population_experimentid_fkey FOREIGN KEY (experimentid)
        REFERENCES public.experiment (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE TABLE public.carddefinition
(
    name character varying(200) COLLATE pg_catalog."default" NOT NULL,
    power integer NOT NULL,
    cost integer NOT NULL,
    CONSTRAINT carddefinition_pkey PRIMARY KEY (name)
);

CREATE TABLE public.population_carddefinition
(
    populationid uuid NOT NULL,
    carddefinitionname varchar(200) COLLATE pg_catalog."default" NOT NULL,
    fixed boolean NOT NULL,
    CONSTRAINT population_carddefinition_pkey PRIMARY KEY (populationid, carddefinitionname),
    CONSTRAINT population_carddefinition_populationid_fkey FOREIGN KEY (populationid)
        REFERENCES public.population (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT population_carddefinition_carddefinitionname_fkey FOREIGN KEY (carddefinitionname)
        REFERENCES public.carddefinition (name) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE TABLE public.item
(
    id uuid NOT NULL,
    controller character varying(200) COLLATE pg_catalog."default" NOT NULL,
    firstparentid uuid,
    secondparentid uuid,
    CONSTRAINT item_pkey PRIMARY KEY (id),
    CONSTRAINT item_firstparentid_fkey FOREIGN KEY (firstparentid)
        REFERENCES public.item (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT item_secondparentid_fkey FOREIGN KEY (secondparentid)
        REFERENCES public.item (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE TABLE public.item_carddefinition
(
    itemid uuid NOT NULL,
    carddefinitionname character varying(200) COLLATE pg_catalog."default" NOT NULL,
    cardorder integer NOT NULL,
    CONSTRAINT item_carddefinition_pkey PRIMARY KEY (itemid, carddefinitionname),
    CONSTRAINT item_carddefinition_carddefinitionname_fkey FOREIGN KEY (carddefinitionname)
        REFERENCES public.carddefinition (name) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT item_carddefinition_itemid_fkey FOREIGN KEY (itemid)
        REFERENCES public.item (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE TABLE public.population_item
(
    populationid uuid NOT NULL,
    itemid uuid NOT NULL,
    generation integer NOT NULL,
    CONSTRAINT population_item_pkey PRIMARY KEY (populationid, itemid, generation),
    CONSTRAINT population_item_itemid_fkey FOREIGN KEY (itemid)
        REFERENCES public.item (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT population_item_populationid_fkey FOREIGN KEY (populationid)
        REFERENCES public.population (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);