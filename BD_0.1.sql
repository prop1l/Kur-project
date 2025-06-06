PGDMP           
            }            postgres    14.13    16.3 Z    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    13754    postgres    DATABASE     |   CREATE DATABASE postgres WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';
    DROP DATABASE postgres;
                postgres    false            �           0    0    DATABASE postgres    COMMENT     N   COMMENT ON DATABASE postgres IS 'default administrative connection database';
                   postgres    false    3803                        2615    27794    kur    SCHEMA        CREATE SCHEMA kur;
    DROP SCHEMA kur;
                postgres    false            �            1259    27837    groups    TABLE     _   CREATE TABLE kur.groups (
    group_id integer NOT NULL,
    speciality_id integer NOT NULL
);
    DROP TABLE kur.groups;
       kur         heap    postgres    false    8            �            1259    27836    groups_group_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.groups_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE kur.groups_group_id_seq;
       kur          postgres    false    8    245            �           0    0    groups_group_id_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE kur.groups_group_id_seq OWNED BY kur.groups.group_id;
          kur          postgres    false    244            �            1259    27868    ratings    TABLE     �   CREATE TABLE kur.ratings (
    rating_id integer NOT NULL,
    group_id integer NOT NULL,
    teacher_id integer NOT NULL,
    assessment numeric(3,1)
);
    DROP TABLE kur.ratings;
       kur         heap    postgres    false    8            �            1259    27867    ratings_rating_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.ratings_rating_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 )   DROP SEQUENCE kur.ratings_rating_id_seq;
       kur          postgres    false    251    8            �           0    0    ratings_rating_id_seq    SEQUENCE OWNED BY     I   ALTER SEQUENCE kur.ratings_rating_id_seq OWNED BY kur.ratings.rating_id;
          kur          postgres    false    250            �            1259    27804    roles    TABLE     g   CREATE TABLE kur.roles (
    role_id integer NOT NULL,
    role_name character varying(50) NOT NULL
);
    DROP TABLE kur.roles;
       kur         heap    postgres    false    8            �            1259    27803    roles_role_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.roles_role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 %   DROP SEQUENCE kur.roles_role_id_seq;
       kur          postgres    false    241    8            �           0    0    roles_role_id_seq    SEQUENCE OWNED BY     A   ALTER SEQUENCE kur.roles_role_id_seq OWNED BY kur.roles.role_id;
          kur          postgres    false    240            �            1259    27830    specialities    TABLE     �   CREATE TABLE kur.specialities (
    speciality_id integer NOT NULL,
    spec_name character varying(100) NOT NULL,
    date_formation timestamp without time zone,
    date_disbandment timestamp without time zone
);
    DROP TABLE kur.specialities;
       kur         heap    postgres    false    8            �            1259    27829    specialities_speciality_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.specialities_speciality_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 2   DROP SEQUENCE kur.specialities_speciality_id_seq;
       kur          postgres    false    8    243            �           0    0    specialities_speciality_id_seq    SEQUENCE OWNED BY     [   ALTER SEQUENCE kur.specialities_speciality_id_seq OWNED BY kur.specialities.speciality_id;
          kur          postgres    false    242            �            1259    27856    subjects    TABLE     �   CREATE TABLE kur.subjects (
    subject_id integer NOT NULL,
    subject_name character varying(100) NOT NULL,
    teacher_id integer NOT NULL
);
    DROP TABLE kur.subjects;
       kur         heap    postgres    false    8            �            1259    27855    subjects_subject_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.subjects_subject_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 +   DROP SEQUENCE kur.subjects_subject_id_seq;
       kur          postgres    false    249    8            �           0    0    subjects_subject_id_seq    SEQUENCE OWNED BY     M   ALTER SEQUENCE kur.subjects_subject_id_seq OWNED BY kur.subjects.subject_id;
          kur          postgres    false    248            �            1259    27849    teachers    TABLE     �   CREATE TABLE kur.teachers (
    teacher_id integer NOT NULL,
    first_name character varying(100) NOT NULL,
    middle_name character varying(100),
    last_name character varying(100) NOT NULL,
    date_birth date NOT NULL
);
    DROP TABLE kur.teachers;
       kur         heap    postgres    false    8            �            1259    27848    teachers_teacher_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.teachers_teacher_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 +   DROP SEQUENCE kur.teachers_teacher_id_seq;
       kur          postgres    false    8    247            �           0    0    teachers_teacher_id_seq    SEQUENCE OWNED BY     M   ALTER SEQUENCE kur.teachers_teacher_id_seq OWNED BY kur.teachers.teacher_id;
          kur          postgres    false    246            c           1259    31613    tokens    TABLE     [   CREATE TABLE kur.tokens (
    id_user integer,
    token character varying(25) NOT NULL
);
    DROP TABLE kur.tokens;
       kur         heap    postgres    false    8            e           1259    31651    tokenss    TABLE     �   CREATE TABLE kur.tokenss (
    token_id integer NOT NULL,
    token_value character varying(255) NOT NULL,
    users_id integer NOT NULL,
    created_at timestamp with time zone DEFAULT now(),
    expires_at timestamp with time zone NOT NULL
);
    DROP TABLE kur.tokenss;
       kur         heap    postgres    false    8            d           1259    31650    tokenss_token_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.tokenss_token_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE kur.tokenss_token_id_seq;
       kur          postgres    false    8    357            �           0    0    tokenss_token_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE kur.tokenss_token_id_seq OWNED BY kur.tokenss.token_id;
          kur          postgres    false    356                       1259    28155 	   user_info    TABLE     �   CREATE TABLE kur.user_info (
    user_id integer,
    first_name character varying(255),
    second_name character varying(255),
    last_name character varying(255)
);
    DROP TABLE kur.user_info;
       kur         heap    postgres    false    8                       1259    28166 
   user_roles    TABLE        CREATE TABLE kur.user_roles (
    user_role_id integer NOT NULL,
    user_id integer NOT NULL,
    role_id integer NOT NULL
);
    DROP TABLE kur.user_roles;
       kur         heap    postgres    false    8                       1259    28165    user_roles_user_role_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.user_roles_user_role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 /   DROP SEQUENCE kur.user_roles_user_role_id_seq;
       kur          postgres    false    8    277            �           0    0    user_roles_user_role_id_seq    SEQUENCE OWNED BY     U   ALTER SEQUENCE kur.user_roles_user_role_id_seq OWNED BY kur.user_roles.user_role_id;
          kur          postgres    false    276                       1259    28136    users    TABLE       CREATE TABLE kur.users (
    user_id integer NOT NULL,
    login character varying(255),
    email character varying(255),
    password text,
    date_created date DEFAULT CURRENT_DATE,
    date_update date DEFAULT CURRENT_DATE,
    is_email_confirmed boolean DEFAULT false NOT NULL
);
    DROP TABLE kur.users;
       kur         heap    postgres    false    8                       1259    28135    users_user_id_seq    SEQUENCE     �   CREATE SEQUENCE kur.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 %   DROP SEQUENCE kur.users_user_id_seq;
       kur          postgres    false    274    8            �           0    0    users_user_id_seq    SEQUENCE OWNED BY     A   ALTER SEQUENCE kur.users_user_id_seq OWNED BY kur.users.user_id;
          kur          postgres    false    273            �           2604    27840    groups group_id    DEFAULT     l   ALTER TABLE ONLY kur.groups ALTER COLUMN group_id SET DEFAULT nextval('kur.groups_group_id_seq'::regclass);
 ;   ALTER TABLE kur.groups ALTER COLUMN group_id DROP DEFAULT;
       kur          postgres    false    245    244    245            �           2604    27871    ratings rating_id    DEFAULT     p   ALTER TABLE ONLY kur.ratings ALTER COLUMN rating_id SET DEFAULT nextval('kur.ratings_rating_id_seq'::regclass);
 =   ALTER TABLE kur.ratings ALTER COLUMN rating_id DROP DEFAULT;
       kur          postgres    false    250    251    251            �           2604    27807    roles role_id    DEFAULT     h   ALTER TABLE ONLY kur.roles ALTER COLUMN role_id SET DEFAULT nextval('kur.roles_role_id_seq'::regclass);
 9   ALTER TABLE kur.roles ALTER COLUMN role_id DROP DEFAULT;
       kur          postgres    false    240    241    241            �           2604    27833    specialities speciality_id    DEFAULT     �   ALTER TABLE ONLY kur.specialities ALTER COLUMN speciality_id SET DEFAULT nextval('kur.specialities_speciality_id_seq'::regclass);
 F   ALTER TABLE kur.specialities ALTER COLUMN speciality_id DROP DEFAULT;
       kur          postgres    false    242    243    243            �           2604    27859    subjects subject_id    DEFAULT     t   ALTER TABLE ONLY kur.subjects ALTER COLUMN subject_id SET DEFAULT nextval('kur.subjects_subject_id_seq'::regclass);
 ?   ALTER TABLE kur.subjects ALTER COLUMN subject_id DROP DEFAULT;
       kur          postgres    false    249    248    249            �           2604    27852    teachers teacher_id    DEFAULT     t   ALTER TABLE ONLY kur.teachers ALTER COLUMN teacher_id SET DEFAULT nextval('kur.teachers_teacher_id_seq'::regclass);
 ?   ALTER TABLE kur.teachers ALTER COLUMN teacher_id DROP DEFAULT;
       kur          postgres    false    247    246    247            �           2604    31654    tokenss token_id    DEFAULT     n   ALTER TABLE ONLY kur.tokenss ALTER COLUMN token_id SET DEFAULT nextval('kur.tokenss_token_id_seq'::regclass);
 <   ALTER TABLE kur.tokenss ALTER COLUMN token_id DROP DEFAULT;
       kur          postgres    false    356    357    357            �           2604    28169    user_roles user_role_id    DEFAULT     |   ALTER TABLE ONLY kur.user_roles ALTER COLUMN user_role_id SET DEFAULT nextval('kur.user_roles_user_role_id_seq'::regclass);
 C   ALTER TABLE kur.user_roles ALTER COLUMN user_role_id DROP DEFAULT;
       kur          postgres    false    277    276    277            �           2604    28139    users user_id    DEFAULT     h   ALTER TABLE ONLY kur.users ALTER COLUMN user_id SET DEFAULT nextval('kur.users_user_id_seq'::regclass);
 9   ALTER TABLE kur.users ALTER COLUMN user_id DROP DEFAULT;
       kur          postgres    false    274    273    274            �          0    27837    groups 
   TABLE DATA           6   COPY kur.groups (group_id, speciality_id) FROM stdin;
    kur          postgres    false    245   yd       �          0    27868    ratings 
   TABLE DATA           K   COPY kur.ratings (rating_id, group_id, teacher_id, assessment) FROM stdin;
    kur          postgres    false    251   �d       �          0    27804    roles 
   TABLE DATA           0   COPY kur.roles (role_id, role_name) FROM stdin;
    kur          postgres    false    241   �d       �          0    27830    specialities 
   TABLE DATA           _   COPY kur.specialities (speciality_id, spec_name, date_formation, date_disbandment) FROM stdin;
    kur          postgres    false    243   �d       �          0    27856    subjects 
   TABLE DATA           E   COPY kur.subjects (subject_id, subject_name, teacher_id) FROM stdin;
    kur          postgres    false    249   �d       �          0    27849    teachers 
   TABLE DATA           [   COPY kur.teachers (teacher_id, first_name, middle_name, last_name, date_birth) FROM stdin;
    kur          postgres    false    247   �d       �          0    31613    tokens 
   TABLE DATA           -   COPY kur.tokens (id_user, token) FROM stdin;
    kur          postgres    false    355   �d       �          0    31651    tokenss 
   TABLE DATA           W   COPY kur.tokenss (token_id, token_value, users_id, created_at, expires_at) FROM stdin;
    kur          postgres    false    357   e       �          0    28155 	   user_info 
   TABLE DATA           M   COPY kur.user_info (user_id, first_name, second_name, last_name) FROM stdin;
    kur          postgres    false    275   !e       �          0    28166 
   user_roles 
   TABLE DATA           A   COPY kur.user_roles (user_role_id, user_id, role_id) FROM stdin;
    kur          postgres    false    277   6e       �          0    28136    users 
   TABLE DATA           l   COPY kur.users (user_id, login, email, password, date_created, date_update, is_email_confirmed) FROM stdin;
    kur          postgres    false    274   Ke       �           0    0    groups_group_id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('kur.groups_group_id_seq', 1, false);
          kur          postgres    false    244            �           0    0    ratings_rating_id_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('kur.ratings_rating_id_seq', 1, false);
          kur          postgres    false    250            �           0    0    roles_role_id_seq    SEQUENCE SET     =   SELECT pg_catalog.setval('kur.roles_role_id_seq', 1, false);
          kur          postgres    false    240            �           0    0    specialities_speciality_id_seq    SEQUENCE SET     J   SELECT pg_catalog.setval('kur.specialities_speciality_id_seq', 1, false);
          kur          postgres    false    242            �           0    0    subjects_subject_id_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('kur.subjects_subject_id_seq', 1, false);
          kur          postgres    false    248            �           0    0    teachers_teacher_id_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('kur.teachers_teacher_id_seq', 1, false);
          kur          postgres    false    246            �           0    0    tokenss_token_id_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('kur.tokenss_token_id_seq', 28, true);
          kur          postgres    false    356            �           0    0    user_roles_user_role_id_seq    SEQUENCE SET     G   SELECT pg_catalog.setval('kur.user_roles_user_role_id_seq', 1, false);
          kur          postgres    false    276            �           0    0    users_user_id_seq    SEQUENCE SET     =   SELECT pg_catalog.setval('kur.users_user_id_seq', 61, true);
          kur          postgres    false    273                       2606    27842    groups groups_pkey 
   CONSTRAINT     S   ALTER TABLE ONLY kur.groups
    ADD CONSTRAINT groups_pkey PRIMARY KEY (group_id);
 9   ALTER TABLE ONLY kur.groups DROP CONSTRAINT groups_pkey;
       kur            postgres    false    245                       2606    27873    ratings ratings_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY kur.ratings
    ADD CONSTRAINT ratings_pkey PRIMARY KEY (rating_id);
 ;   ALTER TABLE ONLY kur.ratings DROP CONSTRAINT ratings_pkey;
       kur            postgres    false    251            �           2606    27809    roles roles_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY kur.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (role_id);
 7   ALTER TABLE ONLY kur.roles DROP CONSTRAINT roles_pkey;
       kur            postgres    false    241                       2606    27811    roles roles_role_name_key 
   CONSTRAINT     V   ALTER TABLE ONLY kur.roles
    ADD CONSTRAINT roles_role_name_key UNIQUE (role_name);
 @   ALTER TABLE ONLY kur.roles DROP CONSTRAINT roles_role_name_key;
       kur            postgres    false    241                       2606    27835    specialities specialities_pkey 
   CONSTRAINT     d   ALTER TABLE ONLY kur.specialities
    ADD CONSTRAINT specialities_pkey PRIMARY KEY (speciality_id);
 E   ALTER TABLE ONLY kur.specialities DROP CONSTRAINT specialities_pkey;
       kur            postgres    false    243                       2606    27861    subjects subjects_pkey 
   CONSTRAINT     Y   ALTER TABLE ONLY kur.subjects
    ADD CONSTRAINT subjects_pkey PRIMARY KEY (subject_id);
 =   ALTER TABLE ONLY kur.subjects DROP CONSTRAINT subjects_pkey;
       kur            postgres    false    249            	           2606    27854    teachers teachers_pkey 
   CONSTRAINT     Y   ALTER TABLE ONLY kur.teachers
    ADD CONSTRAINT teachers_pkey PRIMARY KEY (teacher_id);
 =   ALTER TABLE ONLY kur.teachers DROP CONSTRAINT teachers_pkey;
       kur            postgres    false    247                       2606    31617    tokens tokens_token_key 
   CONSTRAINT     P   ALTER TABLE ONLY kur.tokens
    ADD CONSTRAINT tokens_token_key UNIQUE (token);
 >   ALTER TABLE ONLY kur.tokens DROP CONSTRAINT tokens_token_key;
       kur            postgres    false    355                       2606    31657    tokenss tokenss_pkey 
   CONSTRAINT     U   ALTER TABLE ONLY kur.tokenss
    ADD CONSTRAINT tokenss_pkey PRIMARY KEY (token_id);
 ;   ALTER TABLE ONLY kur.tokenss DROP CONSTRAINT tokenss_pkey;
       kur            postgres    false    357                       2606    31659    tokenss tokenss_token_value_key 
   CONSTRAINT     ^   ALTER TABLE ONLY kur.tokenss
    ADD CONSTRAINT tokenss_token_value_key UNIQUE (token_value);
 F   ALTER TABLE ONLY kur.tokenss DROP CONSTRAINT tokenss_token_value_key;
       kur            postgres    false    357                       2606    28171    user_roles user_roles_pkey 
   CONSTRAINT     _   ALTER TABLE ONLY kur.user_roles
    ADD CONSTRAINT user_roles_pkey PRIMARY KEY (user_role_id);
 A   ALTER TABLE ONLY kur.user_roles DROP CONSTRAINT user_roles_pkey;
       kur            postgres    false    277                       2606    28149    users users_email_key 
   CONSTRAINT     N   ALTER TABLE ONLY kur.users
    ADD CONSTRAINT users_email_key UNIQUE (email);
 <   ALTER TABLE ONLY kur.users DROP CONSTRAINT users_email_key;
       kur            postgres    false    274                       2606    28147    users users_login_key 
   CONSTRAINT     N   ALTER TABLE ONLY kur.users
    ADD CONSTRAINT users_login_key UNIQUE (login);
 <   ALTER TABLE ONLY kur.users DROP CONSTRAINT users_login_key;
       kur            postgres    false    274                       2606    28145    users users_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY kur.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);
 7   ALTER TABLE ONLY kur.users DROP CONSTRAINT users_pkey;
       kur            postgres    false    274                       1259    27885    idx_specialities_spec_name    INDEX     U   CREATE INDEX idx_specialities_spec_name ON kur.specialities USING btree (spec_name);
 +   DROP INDEX kur.idx_specialities_spec_name;
       kur            postgres    false    243            
           1259    27887    idx_subjects_subject_name    INDEX     S   CREATE INDEX idx_subjects_subject_name ON kur.subjects USING btree (subject_name);
 *   DROP INDEX kur.idx_subjects_subject_name;
       kur            postgres    false    249                       1259    27886    idx_teachers_last_name    INDEX     M   CREATE INDEX idx_teachers_last_name ON kur.teachers USING btree (last_name);
 '   DROP INDEX kur.idx_teachers_last_name;
       kur            postgres    false    247                       2606    27843     groups groups_speciality_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY kur.groups
    ADD CONSTRAINT groups_speciality_id_fkey FOREIGN KEY (speciality_id) REFERENCES kur.specialities(speciality_id);
 G   ALTER TABLE ONLY kur.groups DROP CONSTRAINT groups_speciality_id_fkey;
       kur          postgres    false    245    3588    243                       2606    27874    ratings ratings_group_id_fkey    FK CONSTRAINT     ~   ALTER TABLE ONLY kur.ratings
    ADD CONSTRAINT ratings_group_id_fkey FOREIGN KEY (group_id) REFERENCES kur.groups(group_id);
 D   ALTER TABLE ONLY kur.ratings DROP CONSTRAINT ratings_group_id_fkey;
       kur          postgres    false    245    251    3590                        2606    27879    ratings ratings_teacher_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY kur.ratings
    ADD CONSTRAINT ratings_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES kur.teachers(teacher_id);
 F   ALTER TABLE ONLY kur.ratings DROP CONSTRAINT ratings_teacher_id_fkey;
       kur          postgres    false    3593    251    247                       2606    27862 !   subjects subjects_teacher_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY kur.subjects
    ADD CONSTRAINT subjects_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES kur.teachers(teacher_id);
 H   ALTER TABLE ONLY kur.subjects DROP CONSTRAINT subjects_teacher_id_fkey;
       kur          postgres    false    3593    247    249            $           2606    31618    tokens tokens_id_user_fkey    FK CONSTRAINT     x   ALTER TABLE ONLY kur.tokens
    ADD CONSTRAINT tokens_id_user_fkey FOREIGN KEY (id_user) REFERENCES kur.users(user_id);
 A   ALTER TABLE ONLY kur.tokens DROP CONSTRAINT tokens_id_user_fkey;
       kur          postgres    false    3604    274    355            %           2606    31660    tokenss tokenss_users_id_fkey    FK CONSTRAINT     |   ALTER TABLE ONLY kur.tokenss
    ADD CONSTRAINT tokenss_users_id_fkey FOREIGN KEY (users_id) REFERENCES kur.users(user_id);
 D   ALTER TABLE ONLY kur.tokenss DROP CONSTRAINT tokenss_users_id_fkey;
       kur          postgres    false    357    274    3604            !           2606    28160     user_info user_info_user_id_fkey    FK CONSTRAINT     ~   ALTER TABLE ONLY kur.user_info
    ADD CONSTRAINT user_info_user_id_fkey FOREIGN KEY (user_id) REFERENCES kur.users(user_id);
 G   ALTER TABLE ONLY kur.user_info DROP CONSTRAINT user_info_user_id_fkey;
       kur          postgres    false    3604    274    275            "           2606    28177 "   user_roles user_roles_role_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY kur.user_roles
    ADD CONSTRAINT user_roles_role_id_fkey FOREIGN KEY (role_id) REFERENCES kur.roles(role_id);
 I   ALTER TABLE ONLY kur.user_roles DROP CONSTRAINT user_roles_role_id_fkey;
       kur          postgres    false    277    3583    241            #           2606    28172 "   user_roles user_roles_user_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY kur.user_roles
    ADD CONSTRAINT user_roles_user_id_fkey FOREIGN KEY (user_id) REFERENCES kur.users(user_id);
 I   ALTER TABLE ONLY kur.user_roles DROP CONSTRAINT user_roles_user_id_fkey;
       kur          postgres    false    277    274    3604            �      \.


      �      \.


      �      \.


      �      \.


      �      \.


      �      \.


      �      \.


      �      \.


      �      \.


      �      \.


      �   \   25	1	1	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-14	2025-04-14	f
 b   47	144	12344	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqunCsnvz490zxe4PkIlXuYWJr.NY9lOVi	2025-04-19	2025-04-19	f
 `   49	qqw	qqw	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqudjH607WqtkoAVl8hr6mEl5nsGzIWtHW	2025-04-19	2025-04-19	f
 `   52	234	453	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-19	2025-04-19	f
 i   54	asdqweq	13123asd	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-19	2025-04-19	f
 l   55	asdqweqwe13	1231asd	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-19	2025-04-19	f
 h   56	213asd	q1231asd	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-19	2025-04-19	f
 k   57	asdfsadfw	adasfsdf	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-19	2025-04-19	f
 r   58	123фыв	йцу123фыв	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-19	2025-04-19	f
 f   59	asdqwe	qweasd	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-19	2025-04-19	t
 c   60	qwee	qweee	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-22	2025-04-22	t
 b   61	2434	2344	$2b$10$Y/Bk/Ct0KG9pGW49AMPKqusoq8KJVQ.6wANUsZs.gIxN2DIykgQHm	2025-04-22	2025-04-22	t
    \.


     